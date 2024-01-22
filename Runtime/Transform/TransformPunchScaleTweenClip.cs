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
    [DisplayName("Punch Scale Tween")]
    public class TransformPunchScaleTweenClip : TweenClip<Transform>
    {
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionVector3 Punch = new TweenTimelineExpressionVector3Constant(Vector3.one);
        
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionInt Vibrato = new TweenTimelineExpressionIntConstant(10);
        
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Elasticity = new TweenTimelineExpressionFloatConstant(1f);
        
        public EaseOrCurve Ease;

        /// <inheritdoc/>
        public override Tween CreateTween(TweenClipInfo<Transform> info)
        {
            return info.Target.DOPunchScale(Punch.Evaluate(info.Parameter), info.Duration, Vibrato.Evaluate(info.Parameter),
                    Elasticity.Evaluate(info.Parameter))
                .SetEase(Ease);
        }
    }
}