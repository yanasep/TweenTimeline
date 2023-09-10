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
        protected override TweenBehaviour<RectTransform> Template => _behaviour;
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

        /// <inheritdoc/>
        public override Tween GetTween()
        {
            Vector3 startValue = Vector3.zero;
            if (SpecifyValue)
            {
                startValue = Value.GetValue(Parameter);
            }
            
            Tween tween = PositionType switch
            {
                RectTransformTweenPositionType.Position => Target.DOMove(SpecifyValue ? startValue : Target.position, Duration),
                RectTransformTweenPositionType.LocalPosition =>  Target.DOLocalMove(SpecifyValue ? startValue : Target.localPosition, Duration),
                RectTransformTweenPositionType.AnchoredPosition =>  Target.DOAnchorPos(SpecifyValue ? startValue : Target.anchoredPosition, Duration),
                _ => throw new ArgumentOutOfRangeException()
            };

            tween.SetEase(Ease.Linear);
            return tween;
        }
    }
}