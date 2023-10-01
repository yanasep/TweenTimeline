using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// スケールTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Scale Tween")]
    public class TransformScaleTweenClip : TweenClip<Transform, TransformScaleTweenBehaviour>
    {
    }

    [Serializable]
    public class TransformScaleTweenBehaviour : TweenBehaviour<Transform>
    {
        [SerializeField] private TweenTimelineField<Vector3> endValue = new(Vector3.one);
        
        [SerializeField] private TweenTimelineField<Ease> ease;

        private Vector3 _start;

        /// <inheritdoc/>
        public override void Start()
        {
            _start = Target.localScale;
        }

        public override void Update(double localTime)
        {
            Target.localScale = DOVirtual.EasedValue(_start, endValue.Value, (float)(localTime / Duration), ease.Value);
        }
    }
}