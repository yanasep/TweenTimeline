using System.ComponentModel;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);

            var binding = director.GetGenericBinding(this) as TextMeshProUGUI;
            if (binding == null) return;
            driver.AddFromName<TextMeshProUGUI>(binding.gameObject, "m_text");
        }
    }
}