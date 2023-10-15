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
    public class RectTransformPositionKeepClip : TweenClip<RectTransform, RectTransformPositionKeepBehaviour>
    {
    }

    [Serializable]
    public class RectTransformPositionKeepBehaviour : TweenBehaviour<RectTransform>
    {
        public RectTransformTweenPositionType PositionType;

        public bool SpecifyValue;

        [EnableIf(nameof(SpecifyValue), true)]
        public TweenTimelineFieldVector3 Value;
        
        private Vector3 _start;

        /// <inheritdoc/>
        public override void Start()
        {
            _start = SpecifyValue switch
            {
                true => Value.Value,
                false => Target.GetPosition(PositionType)
            };
        }

        /// <inheritdoc/>
        public override void Update(double localTime)
        {
            Target.SetPosition(PositionType, _start);
        }
    }
}