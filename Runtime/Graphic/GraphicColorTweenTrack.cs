using System;
using System.ComponentModel;
using DG.Tweening;
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
    [DisplayName("Tween/Color Tween Track")]
    [TrackBindingType(typeof(Graphic))]
    [TrackClipType(typeof(GraphicColorTweenClip))]
    public class GraphicColorTweenTrack : GraphicTweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("Grid.FillTool").image as Texture2D;
#endif
        [SerializeField, ExtractContent] private GraphicColorTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<Graphic> template => _behaviour;
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

        public RGBAFlags Enable;

        private Color _originalValue;

        /// <inheritdoc/>
        public override TweenCallback OnStartCallback => () =>
        {
            if (!SetStartValue) return;
            Target.color = Enable.Apply(_originalValue, StartValue);
        };

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