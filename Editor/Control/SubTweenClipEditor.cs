using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    [CustomTimelineEditor(typeof(SubTweenClip))]
    public class SubTweenClipEditor : ClipEditor
    {
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            var asset = (SubTweenClip)clip.asset;
            PlayableDirector binding = null;

            if (TimelineEditor.inspectedDirector != null)
            {
                binding = TimelineEditor.inspectedDirector.GetGenericBinding(track) as PlayableDirector;
            }

            if (binding)
            {
                // update the duration and loop values (used for UI purposes) here
                // so they are tied to the latest gameObject bound
                asset.UpdateDurationAndLoopFlag(binding);

                clip.displayName = binding.name;
            }
        }

        public override void GetSubTimelines(TimelineClip clip, PlayableDirector director, List<PlayableDirector> subTimelines)
        {
            var asset = (SubTweenClip)clip.asset;

            if (director == null)
                return;

            var subDirector = director.GetGenericBinding(clip.GetParentTrack()) as PlayableDirector;
            if (subDirector == null || subDirector == director || subDirector == TimelineEditor.masterDirector)
                return;

            if (asset.OverwriteSet?.TimelineAsset != null)
                subTimelines.Add(subDirector);
        }
    }
}
