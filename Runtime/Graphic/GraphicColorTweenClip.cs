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
    public class GraphicColorTweenClip : TweenClip<Graphic, GraphicColorTweenBehaviour>
    {
    }

    [Serializable]
    public class GraphicColorTweenBehaviour : TweenBehaviour<Graphic>
    {
        public TweenTimelineField<Color> EndValue = new(Color.white);
        public RGBAFlags Enable;
        public TweenTimelineField<Ease> Ease;

        private Color _start;

        public override void Start()
        {
            _start = Target.color;
        }

        /// <inheritdoc/>
        public override void Update(double localTime)
        {
            Target.color = Color.Lerp(_start, Enable.Apply(Target.color, EndValue.Value), DOVirtual.EasedValue(0, 1, (float)(localTime / Duration), Ease.Value));
        }
    }
}