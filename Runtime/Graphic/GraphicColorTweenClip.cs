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

        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<Graphic> info)
        {
            return DOTween.To(() => info.Target.color, x => info.Target.color = Enable.Apply(info.Target.color, x), EndValue.GetValue(info.Parameter), info.Duration).SetEase(Ease);
        }

        public override string GetTweenLog(TweenClipInfo<Graphic> info)
        {
            return $"DOColor({Enable.GetString(EndValue.GetValue(info.Parameter))}, {info.Duration})";
        }
    }
}