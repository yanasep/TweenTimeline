using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// 移動Tweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Position Keep")]
    public class RectTransformPositionKeepClip : TweenClip<RectTransform>
    {
        public RectTransformTweenPositionType PositionType;

        public bool SpecifyValue;

        [EnableIf(nameof(SpecifyValue), true)]
        public TweenTimelineField<Vector3> Value;

        public override TweenCallback GetStartCallback(TweenClipInfo<RectTransform> info)
        {
            if (!SpecifyValue) return null;

            return PositionType switch
            {
                RectTransformTweenPositionType.AnchoredPosition => () => info.Target.anchoredPosition = Value.Value,
                RectTransformTweenPositionType.Position => () => info.Target.position = Value.Value,
                RectTransformTweenPositionType.LocalPosition => () => info.Target.localPosition = Value.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<RectTransform> info)
        {
            Tween tween = PositionType switch
            {
                RectTransformTweenPositionType.AnchoredPosition => info.Target.DOAnchorPos(Vector3.zero, info.Duration).SetRelative(true),
                RectTransformTweenPositionType.Position => info.Target.DOMove(Vector3.zero, info.Duration).SetRelative(true),
                RectTransformTweenPositionType.LocalPosition => info.Target.DOLocalMove(Vector3.zero, info.Duration).SetRelative(true),
                _ => throw new ArgumentOutOfRangeException()
            };

            tween.SetEase(Ease.Linear);
            return tween;
        }

        /// <inheritdoc/>
        public override string GetTweenLog(TweenClipInfo<RectTransform> info)
        {
            return PositionType switch
            {
                RectTransformTweenPositionType.AnchoredPosition => $"DOAnchorPos(Vector3.zero, {info.Duration}).SetRelative(true)",
                RectTransformTweenPositionType.Position => $"DOMove(Vector3.zero, {info.Duration}).SetRelative(true)",
                RectTransformTweenPositionType.LocalPosition => $"DOLocalMove(Vector3.zero, {info.Duration}).SetRelative(true)",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}