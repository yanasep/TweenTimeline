using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// カラーTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Color Tween")]
    public class GraphicColorTweenClip : TweenClip<Graphic>
    {
        [SerializeField, ExtractContent] private GraphicColorTweenBehaviour _behaviour;
        protected override TweenBehaviour<Graphic> Template => _behaviour;
    }

    /// <summary>
    /// カラーTweenビヘイビア
    /// </summary>
    [Serializable]
    public class GraphicColorTweenBehaviour : TweenBehaviour<Graphic>
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionColor EndValue = new TimelineExpressionColorConstant { Value = Color.white };
        public RGBAFlags Enable;
        public Ease Ease;

        private Tween _tween;

        /// <inheritdoc/>
        public override void Start()
        {
            var endVal = Enable.Apply(Target.color, EndValue.GetValue(Parameter));
            _tween = Target.DOColor(endVal, Duration).SetEase(Ease).SetUpdate(UpdateType.Manual);
        }

        /// <inheritdoc/>
        public override void Update(float localTime)
        {
            _tween.Goto(localTime);
        }
    }
}