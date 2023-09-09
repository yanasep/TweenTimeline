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
        protected override TweenBehaviour<Transform> Template => _behaviour;
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

        private Tween _tween;

        /// <inheritdoc/>
        public override void Start()
        {
            switch (PositionType)
            {
                case TransformTweenPositionType.Position:
                    _tween = Target.DOMove(EndValue.GetValue(Parameter), Duration);
                    break;
                case TransformTweenPositionType.LocalPosition:
                    _tween = Target.DOLocalMove(EndValue.GetValue(Parameter), Duration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _tween.SetEase(Ease).SetUpdate(UpdateType.Manual);
            if (IsRelative.GetValue(Parameter)) _tween.SetRelative(true);
        }

        /// <inheritdoc/>
        public override void Update(float localTime)
        {
            _tween.Goto(localTime);
        }
    }
}