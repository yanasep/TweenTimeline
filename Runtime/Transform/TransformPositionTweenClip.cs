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
    public class TransformPositionTweenClip : TweenClip<Transform>
    {
        public TweenTimelineField<TransformTweenPositionType> PositionType;

        public TweenTimelineField<Vector3> EndValue;

        public TweenTimelineField<bool> IsRelative;
        
        public TweenTimelineField<Ease> Ease;

        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<Transform> info)
        {
            Tween tween = PositionType.Value switch
            {
                TransformTweenPositionType.Position => info.Target.DOMove(EndValue.Value, info.Duration),
                TransformTweenPositionType.LocalPosition => info.Target.DOLocalMove(EndValue.Value, info.Duration),
                _ => throw new ArgumentOutOfRangeException()
            };

            tween.SetEase(Ease.Value);
            if (IsRelative.Value) tween.SetRelative(true);

            return tween;   
        }
    }
}