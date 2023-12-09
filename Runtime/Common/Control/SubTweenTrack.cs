using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public bool controlActivation;

        [SerializeField, EnableIf(nameof(controlActivation), true)]
        public ActivationControlPlayable.PostPlaybackState postPlayback = ActivationControlPlayable.PostPlaybackState.Revert;

        /// <inheritdoc/>
        public override Tween CreateTween(CreateTweenArgs args)
        {
            var target = args.Binding as PlayableDirector;
            if (target == null) return null;

            bool hasValidSubTween = GetClips().Any(clip =>
            {
                var subTweenClip = clip.asset as SubTweenClip;
                return subTweenClip != null && subTweenClip.OverwriteSet?.TimelineAsset != null;
            });

            if (!hasValidSubTween) return null;

            var sequence = (Sequence)base.CreateTween(args);
            
            if (controlActivation)
            {
                var activationTween = ActivationTweenTrack.CreateTween(this, target.gameObject, args.Duration, postPlayback);
                sequence.Insert(0, activationTween);
            }
            
            return sequence;
        }

#if UNITY_EDITOR
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = (PlayableDirector)graph.GetResolver();
            var binding = director.GetGenericBinding(this) as PlayableDirector;

            foreach (var clip in GetClips())
            {
                if (clip.asset is SubTweenClip subTweenClip)
                {
                    subTweenClip.Binding = binding;
                    subTweenClip.RootTimelineAsset = timelineAsset;
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
                var subTweenClip = clip.asset as SubTweenClip;
                if (subTweenClip == null)
                    continue;

                if (subTweenClip.OverwriteSet?.TimelineAsset != null)
                {
                    timelinesToPreview.Add(subTweenClip.OverwriteSet.TimelineAsset);
                }
            }

            if (controlActivation)
            {
                driver.AddFromName(binding.gameObject, "m_IsActive");
            }

            SubTweenClip.PreviewDirector(driver, binding, timelinesToPreview);

            s_ProcessedDirectors.Remove(director);

            timelinesToPreview.Clear();
        }
#endif
    }
}