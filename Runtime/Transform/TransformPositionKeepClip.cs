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
        [SerializeField, ExtractContent] private TransformPositionKeepBehaviour _behaviour;
        protected override TweenBehaviour<Transform> Template => _behaviour;
    }

    /// <summary>
    /// 移動Tweenビヘイビア
    /// </summary>
    [Serializable]
    public class TransformPositionKeepBehaviour : TweenBehaviour<Transform>
    {
        public TransformTweenPositionType PositionType;

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
                TransformTweenPositionType.Position => Target.DOMove(SpecifyValue ? startValue : Target.position, Duration),
                TransformTweenPositionType.LocalPosition =>  Target.DOLocalMove(SpecifyValue ? startValue : Target.localPosition, Duration),
                _ => throw new ArgumentOutOfRangeException()
            };

            tween.SetEase(Ease.Linear);
            return tween;
        }
    }
}