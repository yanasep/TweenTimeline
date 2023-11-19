using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// Playable Asset that generates playables for controlling time-related elements on a GameObject.
    /// </summary>
    [Serializable]
    [NotKeyable]
    public class MyControlPlayableAsset : PlayableAsset, IPropertyPreview, ITimelineClipAsset
    {
        [SerializeField] public TimelineAsset timelineAsset;

        /// <summary>
        /// Indicates the active state of the GameObject when Timeline is stopped.
        /// </summary>
        [SerializeField] public ActivationControlPlayable.PostPlaybackState postPlayback = ActivationControlPlayable.PostPlaybackState.Revert;

        double m_Duration = PlayableBinding.DefaultDuration;
        bool m_SupportLoop;

        private static HashSet<PlayableDirector> s_ProcessedDirectors = new HashSet<PlayableDirector>();
        
        public PlayableDirector Binding { get; set; }

        /// <summary>
        /// Returns the duration in seconds needed to play the underlying director or particle system exactly once.
        /// </summary>
        public override double duration => m_Duration;

        /// <summary>
        /// Returns the capabilities of TimelineClips that contain a MyControlPlayableAsset
        /// </summary>
        public ClipCaps clipCaps
        {
            get { return ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | (m_SupportLoop ? ClipCaps.Looping : ClipCaps.None); }
        }

        /// <summary>
        /// Creates the root of a Playable subgraph to control the contents of the game object.
        /// </summary>
        /// <param name="graph">PlayableGraph that will own the playable</param>
        /// <param name="go">The GameObject that triggered the graph build</param>
        /// <returns>The root playable of the subgraph</returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            Playable root = Playable.Null;
            var playables = new List<Playable>();

            m_Duration = PlayableBinding.DefaultDuration;
            m_SupportLoop = false;

            if (Binding != null)
            {
                // update the duration and loop values (used for UI purposes) here
                // so they are tied to the latest gameObject bound
                UpdateDurationAndLoopFlag(Binding);

                if (go == Binding.gameObject)
                {
                    Debug.LogWarningFormat("Control Playable ({0}) is referencing the same PlayableDirector component than the one in which it is playing.", name);
                }

                // TODO: Trackで
                // if (active)
                //     CreateActivationPlayable(sourceObject, graph, playables);

                ConnectDirector(Binding, graph, playables, false);

                // Connect Playables to Generic to Mixer
                root = ConnectPlayablesToMixer(graph, playables);
            }

            if (!root.IsValid())
                root = Playable.Create(graph);

            return root;
        }

        static Playable ConnectPlayablesToMixer(PlayableGraph graph, List<Playable> playables)
        {
            var mixer = Playable.Create(graph, playables.Count);

            for (int i = 0; i != playables.Count; ++i)
            {
                ConnectMixerAndPlayable(graph, mixer, playables[i], i);
            }

            mixer.SetPropagateSetTime(true);

            return mixer;
        }

        void CreateActivationPlayable(GameObject root, PlayableGraph graph,
            List<Playable> outplayables)
        {
            var activation = ActivationControlPlayable.Create(graph, root, postPlayback);
            if (activation.IsValid())
                outplayables.Add(activation);
        }

        void ConnectDirector(PlayableDirector director, PlayableGraph graph,
            List<Playable> outplayables, bool disableSelfReferences)
        {
            if (director.playableAsset != timelineAsset)
            {
                outplayables.Add(DirectorControlPlayable.Create(graph, director));
            }
            // if this self references, disable the director.
            else if (disableSelfReferences)
            {
                director.enabled = false;
            }
        }

        static void ConnectMixerAndPlayable(PlayableGraph graph, Playable mixer, Playable playable,
            int portIndex)
        {
            graph.Connect(playable, 0, mixer, portIndex);
            mixer.SetInputWeight(playable, 1.0f);
        }

        public void UpdateDurationAndLoopFlag(PlayableDirector director)
        {
            const double invalidDuration = double.NegativeInfinity;

            var maxDuration = invalidDuration;
            var supportsLoop = false;

            if (director.playableAsset != null)
            {
                var assetDuration = director.playableAsset.duration;

                if (director.playableAsset is TimelineAsset && assetDuration > 0.0)
                    // Timeline assets report being one tick shorter than they actually are, unless they are empty
                    assetDuration = (double)((DiscreteTime)assetDuration).OneTickAfter();

                maxDuration = Math.Max(maxDuration, assetDuration);
                supportsLoop = supportsLoop || director.extrapolationMode == DirectorWrapMode.Loop;
            }

            m_Duration = double.IsNegativeInfinity(maxDuration) ? PlayableBinding.DefaultDuration : maxDuration;
            m_SupportLoop = supportsLoop;
        }

        /// <inheritdoc/>
        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            // This method is no longer called by Control Tracks.
            if (director == null)
                return;

            // prevent infinite recursion
            if (s_ProcessedDirectors.Contains(director))
                return;
            s_ProcessedDirectors.Add(director);

            // var gameObject = sourceGameObject.Resolve(director);
            // if (gameObject != null)
            // {
            //     if (active)
            //         PreviewActivation(driver, new[] { gameObject });
            //
            //     PreviewDirector(driver, GetComponent<PlayableDirector>(gameObject));
            // }
            s_ProcessedDirectors.Remove(director);
        }

        public static void PreviewActivation(IPropertyCollector driver, GameObject go)
        {
            driver.AddFromName(go, "m_IsActive");
        }

        public static void PreviewDirector(IPropertyCollector driver, PlayableDirector director, IEnumerable<TimelineAsset> timelines)
        {
            foreach (var timeline in timelines)
            {
                timeline.GatherProperties(director, driver);   
            }
        }
    }
}
