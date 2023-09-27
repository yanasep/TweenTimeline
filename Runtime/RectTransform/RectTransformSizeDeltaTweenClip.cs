using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// サイズTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Size Tween")]
    public class RectTransformSizeDeltaTweenClip : TweenClip<RectTransform>
    {
        [SerializeField] private TweenTimelineField<Vector2> endValue = new(new Vector2(100, 100));
        [SerializeField] private TweenTimelineField<Ease> ease;

        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<RectTransform> info)
        {
            return info.Target.DOSizeDelta(endValue.Value, info.Duration).SetEase(ease.Value);
        }
    }
}