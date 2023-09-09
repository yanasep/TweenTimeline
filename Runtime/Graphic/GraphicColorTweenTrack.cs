using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// カラーTweenトラック
    /// </summary>
    [TrackClipType(typeof(GraphicColorTweenClip))]
    [TrackBindingType(typeof(Graphic))]
    [DisplayName("Tween/Color Tween Track")]
    public class GraphicColorTweenTrack : GraphicTweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("Grid.FillTool").image as Texture2D;
#endif
        [SerializeField, ExtractContent] private GraphicColorTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<Graphic> Template => _behaviour;
    }
    
    /// <summary>
    /// カラーTweenミキサー
    /// </summary>
    [Serializable]
    public class GraphicColorTweenMixerBehaviour : TweenMixerBehaviour<Graphic>
    {
        public bool SetStartValue;
        
        [EnableIf(nameof(SetStartValue), true)]
        public Color StartValue = Color.white;

        private Color _originalValue;

        /// <inheritdoc/>
        protected override void OnTrackStart()
        {
            if (!SetStartValue) return;
            Target.color = StartValue;
        }

        /// <inheritdoc/>
        protected override void CacheOriginalState()
        {
            _originalValue = Target.color;
        }

        /// <inheritdoc/>
        protected override void ResetToOriginalState()
        {
            Target.color = _originalValue;
        }
    }
}