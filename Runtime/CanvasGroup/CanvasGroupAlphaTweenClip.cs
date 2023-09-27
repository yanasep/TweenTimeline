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
    public class CanvasGroupAlphaTweenClip : TweenClip<CanvasGroup>
    {
        // public float EndValue = 1f;
        public TweenTimelineField<float> EndValue;
        public TweenTimelineField<Ease> Ease;

        protected override Tween GetTween(TweenClipInfo<CanvasGroup> info)
        {
            return info.Target.DOFade(EndValue.Value, info.Duration).SetEase(Ease.Value);
        }

        public override string GetTweenLog(TweenClipInfo<CanvasGroup> info)
        {
            return $"DOFade({EndValue}, {info.Duration})";
        }
    }
}