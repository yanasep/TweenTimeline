using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// CanvasGroupのAlphaのTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Canvas Group Alpha Tween")]
    public class CanvasGroupAlphaTweenClip : TweenClip<CanvasGroup>
    {
        [SerializeField, ExtractContent] private CanvasGroupAlphaTweenBehaviour _behaviour;
        protected override TweenBehaviour<CanvasGroup> Template => _behaviour;
    }

    /// <summary>
    /// CanvasGroupのAlphaのTweenビヘイビア
    /// </summary>
    [Serializable]
    public class CanvasGroupAlphaTweenBehaviour : TweenBehaviour<CanvasGroup>
    {
        public float EndValue = 1f;
        public Ease Ease;

        /// <inheritdoc/>
        public override Tween GetTween()
        {
            return Target.DOFade(EndValue, Duration).SetEase(Ease);
        }
    }
}