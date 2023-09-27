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

        /// <inheritdoc/>
        protected override TweenCallback GetStartCallback(TweenTrackInfo<Graphic> info)
        {
            if (!SetStartValue) return null;
            return () => info.Target.color = Enable.Apply(info.Target.color, StartValue);;
        }

        protected override string GetStartLog(TweenTrackInfo<Graphic> info)
        {
            return "Set Color: original color (" + Color.white + ") => start color " + Enable.Apply(Color.white, StartValue);
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