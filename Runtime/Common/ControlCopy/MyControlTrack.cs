using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// A Track whose clips control time-related elements on a GameObject.
    /// </summary>
    [TrackClipType(typeof(MyControlPlayableAsset), false)]
    [ExcludeFromPreset]
    // [TimelineHelpURL(typeof(MyControlTrack))]
    [TrackBindingType(typeof(PlayableDirector))]
    public class MyControlTrack : TrackAsset
    {
        public bool controlActive;
        
#if UNITY_EDITOR
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = (PlayableDirector)graph.GetResolver();
            var binding = director.GetGenericBinding(this) as PlayableDirector;
            
            foreach (var clip in GetClips())
            {
                if (clip.asset is MyControlPlayableAsset controlClip)
                {
                    controlClip.Binding = binding;
                }
            }
            return base.CreateTrackMixer(graph, go, inputCount);
        }

        private static readonly HashSet<PlayableDirector> s_ProcessedDirectors = new();

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            if (director == null)
                return;

            // avoid recursion
            if (s_ProcessedDirectors.Contains(director))
                return;

            s_ProcessedDirectors.Add(director);

            var timelinesToPreview = new HashSet<TimelineAsset>();

            var binding = director.GetGenericBinding(this) as PlayableDirector;
            if (binding == null) return;

            foreach (var clip in GetClips())
            {
                var controlClip = clip.asset as MyControlPlayableAsset;
                if (controlClip == null)
                    continue;
                
                if (controlClip.timelineAsset != null)
                {
                    timelinesToPreview.Add(controlClip.timelineAsset);
                }
            }
            
            if (controlActive)
            {
                MyControlPlayableAsset.PreviewActivation(driver, binding.gameObject);
            }
            MyControlPlayableAsset.PreviewDirector(driver, binding, timelinesToPreview);

            s_ProcessedDirectors.Remove(director);

            timelinesToPreview.Clear();
        }

#endif
    }
}
