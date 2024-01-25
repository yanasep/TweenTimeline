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
        protected override TweenCallback GetStartCallback(CreateTweenArgs args)
        {
            var target = (TextMeshProUGUI)args.Binding;
            return () =>
            {
                target.maxVisibleCharacters = 0;
            };
        }

        /// <inheritdoc/>
        protected override TweenCallback GetKillCallback(CreateTweenArgs args)
        {
            var target = (TextMeshProUGUI)args.Binding;
            return () =>
            {
                target.maxVisibleCharacters = 99999;
            };
        }
        
        // NOTE: maxVisibleCharactersはシリアライズされないので、GatherPropertiesはなし
    }
}