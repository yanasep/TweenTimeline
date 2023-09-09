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

        private Tween _tween;

        /// <inheritdoc/>
        public override void Start()
        {
            switch (PositionType)
            {
                case RectTransformTweenPositionType.AnchoredPosition:
                    _tween = Target.DOAnchorPos(EndValue.GetValue(Parameter), Duration);
                    break;
                case RectTransformTweenPositionType.Position:
                    _tween = Target.DOMove(EndValue.GetValue(Parameter), Duration);
                    break;
                case RectTransformTweenPositionType.LocalPosition:
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