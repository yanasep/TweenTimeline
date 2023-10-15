using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// サイズTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Size Tween")]
    public class RectTransformSizeDeltaTweenClip : TweenClip<RectTransform, RectTransformSizeDeltaTweenBehaviour>
    {
    }

    [Serializable]
    public class RectTransformSizeDeltaTweenBehaviour : TweenBehaviour<RectTransform>
    {
        [SerializeField] private TweenTimelineFieldVector2 endValue = new(new Vector2(100, 100));
        [SerializeField] private TweenTimelineField<Ease> ease;

        private Vector2 _start;

        public override void Start()
        {
            _start = Target.sizeDelta;
        }

        /// <inheritdoc/>
        public override void Update(double localTime)
        {
            Target.sizeDelta = DOVirtual.EasedValue(_start, endValue.Value, (float)(localTime / Duration), ease.Value);
        }
    }
}