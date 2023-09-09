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
        [SerializeField, ExtractContent] private RectTransformPunchScaleTweenBehaviour _behaviour;
        protected override TweenBehaviour<Transform> Template => _behaviour;
    }

    /// <summary>
    /// スケールTweenビヘイビア
    /// </summary>
    [Serializable]
    public class RectTransformPunchScaleTweenBehaviour : TweenBehaviour<Transform>
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector3 Punch = new TimelineExpressionVector3Constant();
        
        public Ease Ease;
        public int Vibrato = 10;
        public float Elasticity = 1f;

        private Tween _tween;

        /// <inheritdoc/>
        public override void Start()
        {
            _tween = Target.DOPunchScale(Punch.GetValue(Parameter), Duration, Vibrato, Elasticity).SetEase(Ease).SetUpdate(UpdateType.Manual);
        }

        /// <inheritdoc/>
        public override void Update(float localTime)
        {
            _tween.Goto(localTime);
        }
    }
}