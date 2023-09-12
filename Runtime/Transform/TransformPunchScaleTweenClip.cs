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
    [DisplayName("Punch Scale Tween")]
    public class TransformPunchScaleTweenClip : TweenClip<Transform>
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector3 Punch = new TimelineExpressionVector3Constant();
        
        public Ease Ease;
        public int Vibrato = 10;
        public float Elasticity = 1f;

        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<Transform> info)
        {
            return info.Target.DOPunchScale(Punch.GetValue(info.Parameter), info.Duration, Vibrato, Elasticity).SetEase(Ease);
        }
    }
}