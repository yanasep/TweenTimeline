using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// 移動Tweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Position Tween")]
    public class RectTransformPositionTweenClip : TweenClip<RectTransform, RectTransformPositionTweenBehaviour>
    {
    }

    [Serializable]
    public class RectTransformPositionTweenBehaviour : TweenBehaviour<RectTransform>
    {
        public RectTransformTweenPositionType PositionType;

        public TweenTimelineField<Vector3> EndValue;

        public TweenTimelineField<bool> IsRelative;

        public Ease Ease;

        private Vector3 _startValue;

        public override void Start()
        {
            _startValue = Target.GetPosition(PositionType);
        }

        public override void Update(double localTime)
        {
            var val = DOVirtual.EasedValue(_startValue, EndValue.Value, (float)(localTime / Duration), Ease);
            if (IsRelative.Value) val += _startValue;
            Target.SetPosition(PositionType, val);
        } 
    }
}
