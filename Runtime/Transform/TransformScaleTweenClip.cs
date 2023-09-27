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
    [DisplayName("Scale Tween")]
    public class TransformScaleTweenClip : TweenClip<Transform>
    {
        [SerializeField] private TweenTimelineField<Vector3> endValue = new(Vector3.one);
        
        [SerializeField] private TweenTimelineField<Ease> ease;

        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<Transform> info)
        {
            return info.Target.DOScale(endValue.Value, info.Duration).SetEase(ease.Value);
        }
    }
}