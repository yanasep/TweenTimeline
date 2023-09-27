using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// 移動Tweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Position Tween")]
    public class RectTransformPositionTweenClip : TweenClip<RectTransform>
    {
        public RectTransformTweenPositionType PositionType;

        public TweenTimelineField<Vector3> EndValue;

        public TweenTimelineField<bool> IsRelative;

        public Ease Ease;

        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<RectTransform> info)
        {
            Tween tween;
            switch (PositionType)
            {
                case RectTransformTweenPositionType.AnchoredPosition:
                    tween = info.Target.DOAnchorPos(EndValue.Value, info.Duration);
                    break;
                case RectTransformTweenPositionType.Position:
                    tween = info.Target.DOMove(EndValue.Value, info.Duration);
                    break;
                case RectTransformTweenPositionType.LocalPosition:
                    tween = info.Target.DOLocalMove(EndValue.Value, info.Duration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            tween.SetEase(Ease);
            if (IsRelative.Value) tween.SetRelative(true);

            return tween;
        }

        /// <inheritdoc/>
        public override string GetTweenLog(TweenClipInfo<RectTransform> info)
        {
            string log;

            switch (PositionType)
            {
                case RectTransformTweenPositionType.AnchoredPosition:
                    log = $"DOAnchorPos({EndValue}, {info.Duration:F2}f)";
                    break;
                case RectTransformTweenPositionType.Position:
                    log = $"DOMove({EndValue}, {info.Duration:F2}f)";
                    break;
                case RectTransformTweenPositionType.LocalPosition:
                    log = $"DOLocalMove({EndValue}, {info.Duration:F2}f)";
                    break;
                default:
                    return null;
            }

            if (IsRelative.Value) log += ".SetRelative(true)";
            return log;
        }
    }
}
