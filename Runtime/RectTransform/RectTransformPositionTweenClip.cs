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
    [DisplayName("Position Tween")]
    public class RectTransformPositionTweenClip : TweenClip<RectTransform>
    {
        [SerializeField, ExtractContent] private RectTransformPositionTweenBehaviour _behaviour;
        protected override TweenBehaviour<RectTransform> Template => _behaviour;
    }

    /// <summary>
    /// 移動Tweenビヘイビア
    /// </summary>
    [Serializable]
    public class RectTransformPositionTweenBehaviour : TweenBehaviour<RectTransform>
    {
        public RectTransformTweenPositionType PositionType;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector2 EndValue = new TimelineExpressionVector2Constant();

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionBool IsRelative = new TimelineExpressionBoolConstant { Value = false };
        
        public Ease Ease;

        /// <inheritdoc/>
        public override Tween GetTween()
        {
            Tween tween;
            switch (PositionType)
            {
                case RectTransformTweenPositionType.AnchoredPosition:
                    tween = Target.DOAnchorPos(EndValue.GetValue(Parameter), Duration);
                    break;
                case RectTransformTweenPositionType.Position:
                    tween = Target.DOMove(EndValue.GetValue(Parameter), Duration);
                    break;
                case RectTransformTweenPositionType.LocalPosition:
                    tween = Target.DOLocalMove(EndValue.GetValue(Parameter), Duration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            tween.SetEase(Ease).SetUpdate(UpdateType.Manual);
            if (IsRelative.GetValue(Parameter)) tween.SetRelative(true);

            return tween;
        }
    }
}