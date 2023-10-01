using System;
using System.ComponentModel;
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
    public class GraphicColorTweenTrack : GraphicTweenTrack<GraphicColorTweenMixerBehaviour>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("Grid.FillTool").image as Texture2D;
#endif

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

    [Serializable]
    public class GraphicColorTweenMixerBehaviour : TweenMixerBehaviour<Graphic>
    {
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        public Color StartValue = Color.white;

        public RGBAFlags Enable;

        /// <inheritdoc/>
        protected override void OnStart(Playable playable)
        {
            if (!SetStartValue) return;
            Target.color = Enable.Apply(Target.color, StartValue);
        }
    }
}