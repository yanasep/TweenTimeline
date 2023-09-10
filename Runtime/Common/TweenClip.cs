﻿using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// タイムラインTweenクリップ
    /// </summary>
    public abstract class TweenClip : PlayableAsset
    {
        public object PlayerData { get; set; }
        public TimelineClip Clip { get; set; }

        public abstract Tween GetTween();
    }

    /// <summary>
    /// タイムラインTweenクリップ
    /// </summary>
    [Serializable]
    public abstract class TweenClip<TBinding> : TweenClip, ITimelineClipAsset where TBinding : class
    {
        public virtual ClipCaps clipCaps => ClipCaps.None;
        protected abstract TweenBehaviour<TBinding> Template { get; }

        /// <inheritdoc/>
        public sealed override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            // OnPlayableCreateでTargetを参照できるように、Templateにセットしておく
            Template.Target = PlayerData as TBinding;
            if (Template.Target == null) return default;
            var playable = ScriptPlayable<TweenBehaviour>.Create(graph, Template);
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
            behaviour.Target = PlayerData as TBinding;
            if (owner.TryGetComponent<TweenParameterInjector>(out var parameterContainer))
            {
                behaviour.Parameter = parameterContainer.GetParameter();
            }
        }

        public override Tween GetTween()
        {
            return Template.GetTween();
        }
    }

    /// <summary>
    /// タイムラインTweenビヘイビア
    /// NOTE: ScriptPlayableの型引数に取りたいのでabstractにしていない
    /// </summary>
    [Serializable]
    public class TweenBehaviour : PlayableBehaviour
    {
        public float Duration { get; set; }
        public float StartTime { get; set; }

        /// <summary>
        /// クリップ突入時
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// クリップ再生中の更新
        /// </summary>
        public virtual void Update(float localTime) { }

        /// <summary>
        /// クリップ終了時
        /// </summary>
        public virtual void End() { }
    }

    /// <summary>
    /// タイムラインTweenビヘイビア
    /// </summary>
    [Serializable]
    public abstract class TweenBehaviour<TTweenObj> : TweenBehaviour where TTweenObj : class
    {
        public TTweenObj Target { get; set; }
        public TweenParameter Parameter { get; set; }

        private Tween _tween;

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

        public abstract Tween GetTween();

        public sealed override void Start()
        {
            _tween = GetTween();
        }

        public override void Update(float localTime)
        {
            _tween.Goto(localTime);
        }

        public sealed override void End()
        {
        }
    }
}