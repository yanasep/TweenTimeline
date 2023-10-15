using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

namespace TweenTimeline
{
    /// <summary>
    /// スケールTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Punch Scale Tween")]
    public class TransformPunchScaleTweenClip : TweenClip<Transform, TransformPunchScaleTweenBehaviour>
    {
    }

    [Serializable]
    public class TransformPunchScaleTweenBehaviour : TweenBehaviour<Transform>
    {
        [SerializeField] public TweenTimelineField<Vector3> Punch;
        
        public TweenTimelineField<Ease> Ease;
        public TweenTimelineFieldInt Vibrato = new(10);
        public TweenTimelineFieldFloat Elasticity = new(1f);

        private Tween _tween;

        /// <inheritdoc/>
        public override void Start()
        {
            _tween = Target.DOPunchScale(Punch.Value, (float)Duration, Vibrato.Value, Elasticity.Value).SetEase(Ease.Value)
                .Pause().SetAutoKill(false);
        }

        /// <inheritdoc/>
        public override void Update(double localTime)
        {
            _tween.Goto((float)localTime);
        }

        /// <inheritdoc/>
        public override void OnPlayableDestroy(Playable playable)
        {
            _tween.Kill();
        }
    }
}