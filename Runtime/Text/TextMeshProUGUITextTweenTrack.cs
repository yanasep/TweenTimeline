using System.ComponentModel;
using DG.Tweening;
using TMPro;
using UnityEngine.Timeline;

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
        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(TweenTrackInfo<TextMeshProUGUI> info)
        {
            return () => info.Target.maxVisibleCharacters = 0;
        }

        /// <inheritdoc/>
        public override TweenCallback GetEndCallback(TweenTrackInfo<TextMeshProUGUI> info)
        {
            return () => info.Target.maxVisibleCharacters = 99999;
        }
    }
}