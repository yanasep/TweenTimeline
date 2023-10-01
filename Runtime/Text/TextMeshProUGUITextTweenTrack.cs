using System;
using System.ComponentModel;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// CanvasGroupのAlphaのTweenトラック
    /// </summary>
    [TrackClipType(typeof(TextMeshProUGUITextTweenClip))]
    [TrackBindingType(typeof(TextMeshProUGUI))]
    [DisplayName("Tween/Text Tween Track")]
    public class TextMeshProUGUITextTweenTrack : TextMeshProUGUITweenTrack<TextMeshProUGUITextTweenMixerBehaviour>
    {
    }

    [Serializable]
    public class TextMeshProUGUITextTweenMixerBehaviour : TweenMixerBehaviour<TextMeshProUGUI>
    {
        /// <inheritdoc/>
        protected override void OnStart(Playable playable)
        {
            Target.maxVisibleCharacters = 0;
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            Target.maxVisibleCharacters = 99999;
        }
    }
}