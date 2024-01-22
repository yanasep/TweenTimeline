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
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat EndValue = new TweenTimelineExpressionFloatConstant();

        public EaseOrCurve Ease;

        /// <inheritdoc/>
        public override Tween CreateTween(TweenClipInfo<CanvasGroup> info)
        {
            return DOTween.To(() => info.Target.alpha, val => info.Target.alpha = val, EndValue.Evaluate(info.Parameter), info.Duration)
                .SetEase(Ease);
        }
    }
}
