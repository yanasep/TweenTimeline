using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Playable Asset that generates playables for controlling time-related elements on a GameObject.
    /// </summary>
    [Serializable]
    [NotKeyable]
    public class SubTweenClip : TweenClip<PlayableDirector>, IPropertyPreview
    {
        [SerializeField] public TimelineAsset timelineAsset;

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
        public override ClipCaps clipCaps
        {
            get { return ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | (m_SupportLoop ? ClipCaps.Looping : ClipCaps.None); }
        }
        
         [SerializeField] private ParameterOverwrite<TweenTimelineExpressionInt, int>[] ints;
         [SerializeField] private ParameterOverwrite<TweenTimelineExpressionFloat, float>[] floats;
         [SerializeField] private ParameterOverwrite<TweenTimelineExpressionBool, bool>[] bools;
         [SerializeField] private ParameterOverwrite<TweenTimelineExpressionVector3, Vector3>[] vector3s;
         [SerializeField] private ParameterOverwrite<TweenTimelineExpressionVector2, Vector2>[] vector2s;
         [SerializeField] private ParameterOverwrite<TweenTimelineExpressionColor, Color>[] colors;

         [Serializable]
         private struct ParameterOverwrite<TExpression, TValue> where TExpression : TweenTimelineExpression<TValue>
         {
             public string ParameterName;
             [SerializeReference, SelectableSerializeReference]
             public TExpression Expression;
         }
         
         public override Tween CreateTween(TweenClipInfo<PlayableDirector> info)
         {
             if (timelineAsset == null) return null;
             
             return TweenTimelineUtility.CreateTween(timelineAsset, info.Target, parameter =>
             {
                 Set(parameter.Int, ints, info.Parameter);
                 Set(parameter.Float, floats, info.Parameter);
                 Set(parameter.Bool, bools, info.Parameter);
                 Set(parameter.Vector3, vector3s, info.Parameter);
                 Set(parameter.Vector2, vector2s, info.Parameter);
                 Set(parameter.Color, colors, info.Parameter);
             });
         }

         private void Set<TVal, TExp>(TimelineParameterDictionary<TVal> destParamDic, ParameterOverwrite<TExp, TVal>[] overwrites, 
             TweenParameter parentParameter)
             where TExp : TweenTimelineExpression<TVal>
         {
             foreach (var overwrite in overwrites)
             {
                 if (overwrite.Expression == null) continue;
                 destParamDic.Set(overwrite.ParameterName, overwrite.Expression.Evaluate(parentParameter));
             }
         }

        /// <summary>
        /// Creates the root of a Playable subgraph to control the contents of the game object.
        /// </summary>
        /// <param name="graph">PlayableGraph that will own the playable</param>
        /// <param name="go">The GameObject that triggered the graph build</param>
        /// <returns>The root playable of the subgraph</returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
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
            }

            return Playable.Null;
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
                supportsLoop = director.extrapolationMode == DirectorWrapMode.Loop;
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
            PreviewDirector(driver, director, new[] { timelineAsset });
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
