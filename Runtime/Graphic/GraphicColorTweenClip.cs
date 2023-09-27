using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TweenTimeline
{
    /// <summary>
    /// カラーTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Color Tween")]
    public class GraphicColorTweenClip : TweenClip<Graphic>
    {
        public TweenTimelineField<Color> EndValue = new(Color.white);
        public RGBAFlags Enable;
        public TweenTimelineField<Ease> Ease;

        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<Graphic> info)
        {
            return DOTween.To(() => info.Target.color, x => info.Target.color = Enable.Apply(info.Target.color, x), EndValue.Value, info.Duration).SetEase(Ease.Value);
        }

        public override string GetTweenLog(TweenClipInfo<Graphic> info)
        {
            return $"DOColor({Enable.GetString(EndValue.Value)}, {info.Duration})";
        }
    }
}