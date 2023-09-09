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
                    TransformTweenPositionType.Position => Target.position,
                    TransformTweenPositionType.LocalPosition => Target.localPosition,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        /// <inheritdoc/>
        public override void Update(float localTime)
        {
            switch (PositionType)
            {
                case TransformTweenPositionType.Position:
                    Target.position = _startValue;
                    break;
                case TransformTweenPositionType.LocalPosition:
                    Target.localPosition = _startValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}