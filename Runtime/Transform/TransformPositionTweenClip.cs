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
    public class TransformPositionTweenClip : TweenClip<Transform>
    {
        [SerializeField, ExtractContent] private TransformPositionTweenBehaviour _behaviour;
        protected override TweenBehaviour<Transform> template => _behaviour;
    }

    /// <summary>
    /// 移動Tweenビヘイビア
    /// </summary>
    [Serializable]
    public class TransformPositionTweenBehaviour : TweenBehaviour<Transform>
    {
        public TransformTweenPositionType PositionType;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 EndValue = new TimelineExpressionVector3Constant();

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionBool IsRelative = new TimelineExpressionBoolConstant { Value = false };
        
        public Ease Ease;

        /// <inheritdoc/>
        public override Tween GetTween()
        {
            Tween tween = PositionType switch
            {
                TransformTweenPositionType.Position => Target.DOMove(EndValue.GetValue(Parameter), Duration),
                TransformTweenPositionType.LocalPosition => Target.DOLocalMove(EndValue.GetValue(Parameter), Duration),
                _ => throw new ArgumentOutOfRangeException()
            };

            tween.SetEase(Ease);
            if (IsRelative.GetValue(Parameter)) tween.SetRelative(true);

            return tween;
        }
    }
}