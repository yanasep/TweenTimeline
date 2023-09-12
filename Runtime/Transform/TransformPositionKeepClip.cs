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
        public TransformTweenPositionType PositionType;

        public bool SpecifyValue;
        
        [EnableIf(nameof(SpecifyValue), true)]
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 Value = new TimelineExpressionVector3Constant();

        private Vector3 _startValue;

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(TweenClipInfo<Transform> info)
        {
            return () =>
            {
                if (SpecifyValue)
                {
                    _startValue = Value.GetValue(info.Parameter);
                }
                else
                {
                    _startValue = PositionType switch
                    {
                        TransformTweenPositionType.Position => info.Target.position,
                        TransformTweenPositionType.LocalPosition => info.Target.localPosition,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            };
        }

        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<Transform> info)
        {
            Tween tween = PositionType switch
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