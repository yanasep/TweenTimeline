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
        [SerializeField, ExtractContent] private RectTransformScaleTweenBehaviour _behaviour;
        protected override TweenBehaviour<Transform> template => _behaviour;
    }

    /// <summary>
    /// スケールTweenビヘイビア
    /// </summary>
    [Serializable]
    public class RectTransformScaleTweenBehaviour : TweenBehaviour<Transform>
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector3 EndValue = new TimelineExpressionVector3Constant { Value = new Vector3(1, 1, 1) };
        
        public Ease Ease;

        /// <inheritdoc/>
        public override Tween GetTween()
        {
            return Target.DOScale(EndValue.GetValue(Parameter), Duration).SetEase(Ease);
        }
    }
}