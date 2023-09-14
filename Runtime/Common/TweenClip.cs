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

        public double StartTime { get; set; }
        public double Duration { get; set; }

        private readonly TweenBehaviour template = new();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            // // クリップのビヘイビアは使わない
            // return default;

            if (Binding == null) return default;
            
            var parameterHolder = owner.GetComponent<TweenParameterHolder>();
            var parameter = parameterHolder != null ? parameterHolder.GetParameter() : new TweenParameter();

            var info = new TweenClipInfo<TBinding>
            {
                Target = Binding,
                Duration = (float)Duration,
                Parameter = parameter
            };
            template.Tween = GetTween(info);
            template.StartCallback = GetStartCallback(info);
            template.EndCallback = GetEndCallback(info);
            template.StartTime = StartTime;
            template.Duration = duration;
            return ScriptPlayable<TweenBehaviour>.Create(graph, template);
        }
    }

    /// <summary>
    /// TweenTimelineのクリップのBehaviourのベースクラス
    /// </summary>
    public class TweenBehaviour : PlayableBehaviour
    {
        public Tween Tween { get; set; }
        public TweenCallback StartCallback { get; set; }
        public TweenCallback EndCallback { get; set; }
        public double StartTime { get; set; }
        public double Duration { get; set; }
        
        /// <summary>
        /// クリップ突入時
        /// </summary>
        public void Start()
        {
            StartCallback?.Invoke();
        }

        /// <summary>
        /// クリップ再生中の更新
        /// </summary>
        public void Update(double localTime)
        {
            Tween.GotoWithCallbacks((float)localTime);
        }

        /// <summary>
        /// クリップ終了時
        /// </summary>
        public void End()
        {
            EndCallback?.Invoke();
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
    }
}