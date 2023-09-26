using System;
using System.ComponentModel;
using DG.Tweening;
using TriInspector;
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
        [Title("Title")]
        public float EndValue = 1f;
        public Ease Ease;

        public TweenTimelineFieldFloat Val;
        
        public override Tween GetTween(TweenClipInfo<CanvasGroup> info)
        {
            return info.Target.DOFade(EndValue, info.Duration).SetEase(Ease);
        }

        public override string GetTweenLog(TweenClipInfo<CanvasGroup> info)
        {
            return $"DOFade({EndValue}, {info.Duration})";
        }
    }
}