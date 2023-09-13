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
            // ランタイムにセットされた_endValueを取得する必要あり
            // Func<Color> getEndValue = () => _endValue;
            // return info.Target.DOColor(getEndValue(), info.Duration).SetEase(Ease);
            return info.Target.DOColor(Color.white, info.Duration).SetEase(Ease);
        }

        public override string GetStartLog(TweenClipInfo<Graphic> info)
        {
            return $"Set EndValue: current color ({Color.white}) => end color {Enable.Apply(Color.white, EndValue.GetValue(info.Parameter))}";
        }

        public override string GetTweenLog(TweenClipInfo<Graphic> info)
        {
            return $"DOColor({_endValue}, {info.Duration})";
        }
    }
}