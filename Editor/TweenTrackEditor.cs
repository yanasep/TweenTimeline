using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace TweenTimeline.Editor
{
    /// <summary>
    /// TweenTrackのカスタムエディタ
    /// </summary>
    [CustomTimelineEditor(typeof(TweenTrack))]
    public class TweenTrackEditor : TrackEditor
    {
        /// <inheritdoc/>
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            var tweenTrack = (TweenTrack)track;
            
            return new TrackDrawOptions
            {
                errorText = GetErrorText(track, binding, TrackBindingErrors.All),
                minimumHeight = DefaultTrackHeight,
                trackColor = GetTrackColor(track),
                icon = tweenTrack.Icon
            };
        }
    }
}
