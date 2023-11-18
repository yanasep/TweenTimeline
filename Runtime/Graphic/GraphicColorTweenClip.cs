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
        public TweenTimelineExpressionColor EndValue;
        
        public RGBAFlags Enable;
        public EaseOrCurve Ease;

        public override Tween CreateTween(TweenClipInfo<Graphic> info)
        {
            var target = info.Target;
            return DOTween.To(() => target.color, val => target.color = Enable.Apply(target.color, val), EndValue.Evaluate(info.Parameter),
                info.Duration).SetEase(Ease);
        }
    }
}