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
        protected override TweenBehaviour<Transform> Template => _behaviour;
    }

    /// <summary>
    /// スケールTweenビヘイビア
    /// </summary>
    [Serializable]
    public class RectTransformScaleTweenBehaviour : TweenBehaviour<Transform>
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 EndValue = new TimelineExpressionVector3Constant();
        
        public Ease Ease;

        private Tween _tween;

        /// <inheritdoc/>
        public override void Start()
        {
            _tween = Target.DOScale(EndValue.GetValue(Parameter), Duration).SetEase(Ease).SetUpdate(UpdateType.Manual);
        }

        /// <inheritdoc/>
        public override void Update(float localTime)
        {
            _tween.Goto(localTime);
        }
    }
}