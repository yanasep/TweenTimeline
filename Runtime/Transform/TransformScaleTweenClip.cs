using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// スケールTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Scale Tween")]
    public class TransformScaleTweenClip : TweenClip<Transform>
    {
        [SerializeReference, SelectableSerializeReference]
        private TimelineExpressionVector3 endValue = new TimelineExpressionVector3Constant { Value = new Vector3(1, 1, 1) };
        
        [SerializeField] private Ease ease;

        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<Transform> info)
        {
            return info.Target.DOScale(endValue.GetValue(info.Parameter), info.Duration).SetEase(ease);
        }
    }
}