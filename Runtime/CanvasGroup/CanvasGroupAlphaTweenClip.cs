using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// CanvasGroupのAlphaのTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Canvas Group Alpha Tween")]
    public class CanvasGroupAlphaTweenClip : TweenClip<CanvasGroup, CanvasGroupAlphaTweenBehaviour>
    {
    }

    [Serializable]
    public class CanvasGroupAlphaTweenBehaviour : TweenBehaviour<CanvasGroup>
    {
        public TweenTimelineField<float> EndValue;
        public TweenTimelineField<Ease> Ease;

        private float _start;

        public override void Start()
        {
            _start = Target.alpha;
        }

        public override void Update(double localTime)
        {
            Target.alpha = DOVirtual.EasedValue(_start, EndValue.Value, (float)(localTime / Duration), Ease.Value);
        }
    }
}