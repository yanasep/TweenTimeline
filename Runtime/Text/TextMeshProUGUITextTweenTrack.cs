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
    [TrackClipType(typeof(TextMeshProUGUITextTweenClip))]
    [TrackBindingType(typeof(TextMeshProUGUI))]
    [DisplayName("Tween/Text Tween Track")]
    public class TextMeshProUGUITextTweenTrack : TextMeshProUGUITweenTrack
    {
        [SerializeField, ExtractContent] private TextMeshProUGUITextTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<TextMeshProUGUI> Template => _behaviour;
    }
    
    /// <summary>
    /// CanvasGroupのAlphaのTweenミキサー
    /// </summary>
    [Serializable]
    public class TextMeshProUGUITextTweenMixerBehaviour : TweenMixerBehaviour<TextMeshProUGUI>
    {
        private int _originalVisibleCharacters;

        /// <inheritdoc/>
        protected override void OnTrackStart()
        {
            base.OnTrackStart();
            Target.maxVisibleCharacters = 0;
        }

        /// <inheritdoc/>
        protected override void CacheOriginalState()
        {
            _originalVisibleCharacters = Target.maxVisibleCharacters;
        }

        /// <inheritdoc/>
        protected override void ResetToOriginalState()
        {
            Target.maxVisibleCharacters = _originalVisibleCharacters;
        }
    }
}