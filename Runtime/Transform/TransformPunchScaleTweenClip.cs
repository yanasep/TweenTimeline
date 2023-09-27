using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// スケールTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Punch Scale Tween")]
    public class TransformPunchScaleTweenClip : TweenClip<Transform>
    {
        [SerializeField] public TweenTimelineField<Vector3> Punch;
        
        public TweenTimelineField<Ease> Ease;
        public TweenTimelineField<int> Vibrato = new(10);
        public TweenTimelineField<float> Elasticity = new(1f);

        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<Transform> info)
        {
            return info.Target.DOPunchScale(Punch.Value, info.Duration, Vibrato.Value, Elasticity.Value).SetEase(Ease.Value);
        }
    }
}