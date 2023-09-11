using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace TweenTimeline
{
    /// <summary>
    /// タイムラインTweenクリップ
    /// </summary>
    public abstract class TweenClip : PlayableAsset
    {
        public TimelineClip Clip { get; set; }
        public abstract TweenBehaviour Template { get; }
    }

    /// <summary>
    /// タイムラインTweenクリップ
    /// </summary>
    [Serializable]
    public abstract class TweenClip<TBinding> : TweenClip, ITimelineClipAsset where TBinding : Object
    {
        public virtual ClipCaps clipCaps => ClipCaps.None;
        public sealed override TweenBehaviour Template => template;
        protected abstract TweenBehaviour<TBinding> template { get; }

        /// <inheritdoc/>
        public sealed override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TweenBehaviour>.Create(graph, template);
            SetupBehaviour((TweenBehaviour<TBinding>)playable.GetBehaviour(), owner);
            return playable;
        }

        /// <summary>
        /// ビヘイビア生成時のセットアップ
        /// </summary>
        protected virtual void SetupBehaviour(TweenBehaviour<TBinding> behaviour, GameObject owner)
        {
            behaviour.Duration = (float)Clip.duration;
            behaviour.StartTime = (float)Clip.start;
            if (owner.TryGetComponent<TweenParameterInjector>(out var parameterContainer))
            {
                behaviour.Parameter = parameterContainer.GetParameter();
            }
        }
    }

    /// <summary>
    /// タイムラインTweenビヘイビア
    /// </summary>
    [Serializable]
    public class TweenBehaviour : PlayableBehaviour
    {
        public float Duration { get; set; }
        public float StartTime { get; set; }

        public virtual Tween GetTween() => null;
        public virtual TweenCallback OnStartCallback => null;
        public virtual TweenCallback OnEndCallback => null;
    }

    /// <summary>
    /// タイムラインTweenビヘイビア
    /// NOTE: ScriptPlayableの型引数に取りたいのでabstractにしていない
    /// </summary>
    [Serializable]
    public class TweenBehaviour<TTweenObj> : TweenBehaviour where TTweenObj : Object
    {
        public TweenParameter Parameter { get; set; }

        protected TTweenObj Target { get; private set; }
        private Tween _tween;

        /// <inheritdoc/>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            Target = info.output.GetUserData() as TTweenObj;
            if (Target == null) return;
            Start();
        }

        /// <inheritdoc/>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
            
            if (Target == null) return;

            float t = (float)playable.GetTime();
            Update(t);
        }

        /// <inheritdoc/>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (Target == null) return;
            
            if (info.evaluationType == FrameData.EvaluationType.Playback)
            {
                End();
            }
        }

        public void Start()
        {
            OnStartCallback?.Invoke();
            _tween = GetTween();
        }

        public void Update(float localTime)
        {
            _tween.Goto(localTime);
        }

        public void End()
        {
            OnEndCallback?.Invoke();
        }
    }
}