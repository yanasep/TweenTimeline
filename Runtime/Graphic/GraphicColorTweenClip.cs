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
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionColor EndValue = new TimelineExpressionColorConstant { Value = Color.white };
        public RGBAFlags Enable;
        public Ease Ease;
        
        private Color _endValue;

        public override TweenCallback GetStartCallback(TweenClipInfo<Graphic> info)
        {
            return () => _endValue = Enable.Apply(info.Target.color, EndValue.GetValue(info.Parameter));
        }

        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<Graphic> info)
        {
            return info.Target.DOColor(_endValue, info.Duration).SetEase(Ease);
        }
    }
}