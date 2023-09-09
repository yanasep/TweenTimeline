using System;
using System.ComponentModel;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// CanvasGroupのAlphaのTweenトラック
    /// </summary>
    [TrackClipType(typeof(TextMeshProUGUICountUpTweenClip))]
    [TrackBindingType(typeof(TextMeshProUGUI))]
    [DisplayName("Tween/Text Count Up Tween Track")]
    public class TextMeshProUGUICountUpTweenTrack : TextMeshProUGUITweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("d_scenevis_visible_hover").image as Texture2D;
#endif
        [SerializeField, ExtractContent] private TextMeshProUGUICountUpTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<TextMeshProUGUI> Template => _behaviour;
    }
    
    /// <summary>
    /// CanvasGroupのAlphaのTweenミキサー
    /// </summary>
    [Serializable]
    public class TextMeshProUGUICountUpTweenMixerBehaviour : TweenMixerBehaviour<TextMeshProUGUI>
    {
        private string _originalText;

        /// <inheritdoc/>
        protected override void CacheOriginalState()
        {
            _originalText = Target.text;
        }

        /// <inheritdoc/>
        protected override void ResetToOriginalState()
        {
            Target.text = _originalText;
        }
    }
}