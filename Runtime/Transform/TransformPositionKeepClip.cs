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
        protected override TweenBehaviour<Transform> template => _behaviour;
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
                    TransformTweenPositionType.Position => Target.position,
                    TransformTweenPositionType.LocalPosition => Target.localPosition,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        };

        /// <inheritdoc/>
        public override Tween GetTween()
        {
            Tween tween = PositionType switch
            {
                TransformTweenPositionType.Position => Target.DOMove(SpecifyValue ? _startValue : Target.position, Duration),
                TransformTweenPositionType.LocalPosition =>  Target.DOLocalMove(SpecifyValue ? _startValue : Target.localPosition, Duration),
                _ => throw new ArgumentOutOfRangeException()
            };

            tween.SetEase(Ease.Linear);
            return tween;
        }
    }
}