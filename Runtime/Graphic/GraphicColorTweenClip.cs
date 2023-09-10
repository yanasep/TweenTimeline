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

        /// <inheritdoc/>
        public override Tween GetTween()
        {
            var endVal = Enable.Apply(Target.color, EndValue.GetValue(Parameter));
            return Target.DOColor(endVal, Duration).SetEase(Ease);
        }
    }
}