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
        public TransformTweenPositionType PositionType;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 EndValue = new TimelineExpressionVector3Constant();

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionBool IsRelative = new TimelineExpressionBoolConstant { Value = false };
        
        public Ease Ease;

        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<Transform> info)
        {
            Tween tween = PositionType switch
            {
                TransformTweenPositionType.Position => info.Target.DOMove(EndValue.GetValue(info.Parameter), info.Duration),
                TransformTweenPositionType.LocalPosition => info.Target.DOLocalMove(EndValue.GetValue(info.Parameter), info.Duration),
                _ => throw new ArgumentOutOfRangeException()
            };

            tween.SetEase(Ease);
            if (IsRelative.GetValue(info.Parameter)) tween.SetRelative(true);

            return tween;   
        }
    }
}