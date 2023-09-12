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
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 Value = new TimelineExpressionVector3Constant();

        private Vector3 _startValue;

        public override TweenCallback GetStartCallback(TweenClipInfo<RectTransform> info)
        {
            if (SpecifyValue)
            {
                return () => _startValue = Value.GetValue(info.Parameter);
            }
            else
            {
                return () => _startValue = PositionType switch
                {
                    RectTransformTweenPositionType.Position => info.Target.position,
                    RectTransformTweenPositionType.LocalPosition => info.Target.localPosition,
                    RectTransformTweenPositionType.AnchoredPosition => info.Target.anchoredPosition,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        
        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<RectTransform> info)
        {   
            Tween tween = PositionType switch
            {
                RectTransformTweenPositionType.Position => info.Target.DOMove(SpecifyValue ? _startValue : info.Target.position, info.Duration),
                RectTransformTweenPositionType.LocalPosition =>  info.Target.DOLocalMove(SpecifyValue ? _startValue : info.Target.localPosition, info.Duration),
                RectTransformTweenPositionType.AnchoredPosition =>  info.Target.DOAnchorPos(SpecifyValue ? _startValue : info.Target.anchoredPosition, info.Duration),
                _ => throw new ArgumentOutOfRangeException()
            };

            tween.SetEase(Ease.Linear);
            return tween;
        }
    }
}