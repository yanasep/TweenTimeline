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
        public TweenTimelineFieldColor EndValue = new(Color.white);
        public RGBAFlags Enable;
        public TweenTimelineField<Ease> Ease;

        public override Tween CreateTween(TweenClipInfo<Graphic> info)
        {
            var target = info.Target;
            return DOTween.To(() => target.color, val => target.color = Enable.Apply(target.color, val), EndValue.Value, info.Duration).SetEase(Ease.Value);
        }
    }
}