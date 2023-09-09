using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// CanvasGroupのAlphaのTweenトラック
    /// </summary>
    [DisplayName("Tween/Text Count Up Tween Track")]
    [TrackBindingType(typeof(TextMeshProUGUI))]
    [TrackClipType(typeof(TextMeshProUGUICountUpTweenClip))]
    public class TextMeshProUGUICountUpTweenTrack : TextMeshProUGUITweenTrack
    {
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