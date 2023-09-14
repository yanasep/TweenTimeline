using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace TweenTimeline.Editor
{
    [CustomTimelineEditor(typeof(TweenClip))]
    public class TweenClipEditor : ClipEditor
    {
        private const double DefaultClipDuration = 0.5;

        /// <inheritdoc/>
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            clip.duration = DefaultClipDuration;

            base.OnCreate(clip, track, clonedFrom);
        }
    }
}
