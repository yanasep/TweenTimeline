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
    public class TransformPositionKeepClip : TweenClip<Transform>
    {
        public TweenTimelineField<TransformTweenPositionType> PositionType;

        public bool SpecifyValue;

        [EnableIf(nameof(SpecifyValue), true)]
        public TweenTimelineField<Vector3> Value;

        private Vector3 _startValue;

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(TweenClipInfo<Transform> info)
        {
            return () =>
            {
                if (SpecifyValue)
                {
                    _startValue = Value.Value;
                }
                else
                {
                    _startValue = PositionType.Value switch
                    {
                        TransformTweenPositionType.Position => info.Target.position,
                        TransformTweenPositionType.LocalPosition => info.Target.localPosition,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            };
        }

        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<Transform> info)
        {
            Tween tween = PositionType.Value switch
            {
                TransformTweenPositionType.Position => info.Target.DOMove(SpecifyValue ? _startValue : info.Target.position, info.Duration),
                TransformTweenPositionType.LocalPosition =>  info.Target.DOLocalMove(SpecifyValue ? _startValue : info.Target.localPosition, info.Duration),
                _ => throw new ArgumentOutOfRangeException()
            };

            tween.SetEase(Ease.Linear);
            return tween;   
        }
    }
}