using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// カラーTweenトラック
    /// </summary>
    [DisplayName("Tween/Color Tween Track")]
    [TrackBindingType(typeof(Graphic))]
    [TrackClipType(typeof(GraphicColorTweenClip))]
    public class GraphicColorTweenTrack : GraphicTweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("Grid.FillTool").image as Texture2D;
#endif
        
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        public Color StartValue = Color.white;

        public RGBAFlags Enable;

        public override Tween CreateTween(CreateTweenArgs args)
        {
            var tween = base.CreateTween(args);
            if (SetStartValue)
            {
                var target = (Graphic)args.Binding;
                tween.OnStart(() => target.color = Enable.Apply(target.color, StartValue));
            }

            return tween;
        }

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);

#if UNITY_EDITOR
            var binding = director.GetGenericBinding(this) as Graphic;
            if (binding == null) return;
            driver.AddFromName<Graphic>(binding.gameObject, "m_Color");
#endif
        }
    }
}