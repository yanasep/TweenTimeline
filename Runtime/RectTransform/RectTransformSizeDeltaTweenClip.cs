using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// サイズTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Size Tween")]
    public class RectTransformSizeDeltaTweenClip : TweenClip<RectTransform>
    {
        [SerializeReference, SelectableSerializeReference]
        private TimelineExpressionVector2 endValue = new TimelineExpressionVector2Constant { Value = new Vector2(100, 100) };
        [SerializeField] private Ease ease;

        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<RectTransform> info)
        {
            return info.Target.DOSizeDelta(endValue.GetValue(info.Parameter), info.Duration).SetEase(ease);
        }
    }
}