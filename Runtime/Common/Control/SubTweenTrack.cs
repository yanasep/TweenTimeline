using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// A Track whose clips control time-related elements on a GameObject.
    /// </summary>
    [ExcludeFromPreset]
    [DisplayName("Tween/Sub Tween Track")]
    [TrackBindingType(typeof(PlayableDirector))]
    [TrackClipType(typeof(SubTweenClip))]
    public class SubTweenTrack : TweenTrack<PlayableDirector>
    {
        public bool controlActive;

        [SerializeField, EnableIf(nameof(controlActive), true)]
        public ActivationControlPlayable.PostPlaybackState postPlayback = ActivationControlPlayable.PostPlaybackState.Revert;

        /// <inheritdoc/>
        public override Tween CreateTween(CreateTweenArgs args)
        {
            // TODO: activationを追加
            return base.CreateTween(args);
        }

#if UNITY_EDITOR
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = (PlayableDirector)graph.GetResolver();
            var binding = director.GetGenericBinding(this) as PlayableDirector;

            foreach (var clip in GetClips())
            {
                if (clip.asset is SubTweenClip controlClip)
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
                var controlClip = clip.asset as SubTweenClip;
                if (controlClip == null)
                    continue;

                if (controlClip.timelineAsset != null)
                {
                    timelinesToPreview.Add(controlClip.timelineAsset);
                }
            }

            if (controlActive)
            {
                SubTweenClip.PreviewActivation(driver, binding.gameObject);
            }

            SubTweenClip.PreviewDirector(driver, binding, timelinesToPreview);

            s_ProcessedDirectors.Remove(director);

            timelinesToPreview.Clear();
        }
#endif
    }
}