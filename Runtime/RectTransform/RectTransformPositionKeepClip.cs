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
        [SerializeField, ExtractContent] private RectTransformPositionKeepBehaviour _behaviour;
        protected override TweenBehaviour<RectTransform> template => _behaviour;
    }

    /// <summary>
    /// 移動Tweenビヘイビア
    /// </summary>
    [Serializable]
    public class RectTransformPositionKeepBehaviour : TweenBehaviour<RectTransform>
    {
        public RectTransformTweenPositionType PositionType;

        public bool SpecifyValue;
        
        [EnableIf(nameof(SpecifyValue), true)]
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 Value = new TimelineExpressionVector3Constant();

        private Vector3 _startValue;

        public override TweenCallback OnStartCallback => () =>
        {
            if (SpecifyValue)
            {
                _startValue = Value.GetValue(Parameter);
            }
            else
            {
                _startValue = PositionType switch
                {
                    RectTransformTweenPositionType.Position => Target.position,
                    RectTransformTweenPositionType.LocalPosition => Target.localPosition,
                    RectTransformTweenPositionType.AnchoredPosition => Target.anchoredPosition,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        };

        /// <inheritdoc/>
        public override Tween GetTween()
        {   
            Tween tween = PositionType switch
            {
                RectTransformTweenPositionType.Position => Target.DOMove(SpecifyValue ? _startValue : Target.position, Duration),
                RectTransformTweenPositionType.LocalPosition =>  Target.DOLocalMove(SpecifyValue ? _startValue : Target.localPosition, Duration),
                RectTransformTweenPositionType.AnchoredPosition =>  Target.DOAnchorPos(SpecifyValue ? _startValue : Target.anchoredPosition, Duration),
                _ => throw new ArgumentOutOfRangeException()
            };

            tween.SetEase(Ease.Linear);
            return tween;
        }
    }
}