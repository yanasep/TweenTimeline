using System;
using System.ComponentModel;
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

        private Vector3 _startValue;

        /// <inheritdoc/>
        public override void Start()
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
        }

        /// <inheritdoc/>
        public override void Update(float localTime)
        {
            switch (PositionType)
            {
                case RectTransformTweenPositionType.Position:
                    Target.position = _startValue;
                    break;
                case RectTransformTweenPositionType.LocalPosition:
                    Target.localPosition = _startValue;
                    break;
                case RectTransformTweenPositionType.AnchoredPosition:
                    Target.anchoredPosition = _startValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}