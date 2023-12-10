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
        public TweenTimelineExpressionVector3 EndValue = new TweenTimelineExpressionVector3Constant(Vector3.one);
        
        public EaseOrCurve Ease;

        /// <inheritdoc/>
        public override Tween CreateTween(TweenClipInfo<Transform> info)
        {
            return info.Target.DOScale(EndValue.Evaluate(info.Parameter), info.Duration).SetEase(Ease);
        }
    }
}