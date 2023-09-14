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
        public double StartTime { get; set; }
    }

    /// <summary>
    /// タイムラインTweenクリップ
    /// </summary>
    [Serializable]
    public abstract class TweenClip<TBinding> : TweenClip, ITimelineClipAsset where TBinding : Object
    {
        public virtual ClipCaps clipCaps => ClipCaps.None;

        public TBinding Binding { get; set; }

        public abstract Tween GetTween(TweenClipInfo<TBinding> info);
        public virtual TweenCallback GetStartCallback(TweenClipInfo<TBinding> info) => null;
        public virtual TweenCallback GetEndCallback(TweenClipInfo<TBinding> info) => null;
        public virtual string GetStartLog(TweenClipInfo<TBinding> info) => null;
        public virtual string GetTweenLog(TweenClipInfo<TBinding> info) => null;
        public virtual string GetEndLog(TweenClipInfo<TBinding> info) => null;

        private readonly TweenBehaviour<TBinding> template = new();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            // // クリップのビヘイビアは使わない
            // return default;

            if (Binding == null) return default;
            
            var parameterHolder = owner.GetComponent<TweenParameterHolder>();
            var parameter = parameterHolder != null ? parameterHolder.GetParameter() : new TweenParameter();

            template.Info = new TweenClipInfo<TBinding>
            {
                Target = Binding,
                Duration = (float)duration,
                Parameter = parameter
            };
            template.Clip = this;
            // return ScriptPlayable<TweenBehaviour<TBinding>>.Create(graph, template);
            var playable = ScriptPlayable<TweenBehaviour<TBinding>>.Create(graph, template);
            var behaviour = playable.GetBehaviour();
            return playable;
        }
    }

    /// <summary>
    /// TweenTimelineのクリップのBehaviourのベースクラス
    /// </summary>
    public class TweenBehaviour : PlayableBehaviour
    {   
        /// <summary>
        /// クリップ突入時
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// クリップ再生中の更新
        /// </summary>
        public virtual void Update(double localTime) { }

        /// <summary>
        /// クリップ終了時
        /// </summary>
        public virtual void End() { }
    }

    /// <summary>
    /// TweenTimelineのクリップのBehaviourのベースクラス
    /// </summary>
    public class TweenBehaviour<TBinding> : TweenBehaviour where TBinding : Object
    {
        public TweenClipInfo<TBinding> Info { get; set; }
        public TweenClip<TBinding> Clip { get; set; }
        public double StartTime => Clip.StartTime;
        public double Duration => Info.Duration;

        private Tween _tween;

        public override void OnPlayableCreate(Playable playable)
        {
            Debug.Log($"clip behav create");
        }

        /// <inheritdoc/>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            Start();
        }

        /// <inheritdoc/>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);

            float t = (float)playable.GetTime();
            Update(t);
        }

        /// <inheritdoc/>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (info.evaluationType == FrameData.EvaluationType.Playback)
            {
                End();
            }
        }

        public sealed override void Start()
        {
            Clip.GetStartCallback(Info)?.Invoke();
            _tween = Clip.GetTween(Info);
        }

        public sealed override void Update(double localTime)
        {
            _tween.GotoWithCallbacks((float)localTime);
        }

        public sealed override void End()
        {
            Clip.GetEndCallback(Info)?.Invoke();
            _tween.Kill();
        }
    }
}