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
        public TimelineAsset RootTimelineAsset { get; set; }

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

        public ParameterOverwriteSet OverwriteSet;

         [Serializable]
         public class ParameterOverwriteSet
         {
             public List<ParameterOverwrite<TweenTimelineExpressionInt, int>> Ints;
             public List<ParameterOverwrite<TweenTimelineExpressionFloat, float>> Floats;
             public List<ParameterOverwrite<TweenTimelineExpressionBool, bool>> Bools;
             public List<ParameterOverwrite<TweenTimelineExpressionVector3, Vector3>> Vector3s;
             public List<ParameterOverwrite<TweenTimelineExpressionVector2, Vector2>> Vector2s;
             public List<ParameterOverwrite<TweenTimelineExpressionColor, Color>> Colors;
         }

         [Serializable]
         public class ParameterOverwrite
         {
             public string ParameterName;
             /// <summary>インスペクターのリスト表示におけるインデックス</summary>
             public int ViewIndex;
         }

         [Serializable]
         public class ParameterOverwrite<TExpression, TValue> : ParameterOverwrite where TExpression : TweenTimelineExpression<TValue>
         {
             [SerializeReference, SelectableSerializeReference]
             public TExpression Expression;
         }
         
         public override Tween CreateTween(TweenClipInfo<PlayableDirector> info)
         {
             if (timelineAsset == null) return null;
             
             return TweenTimelineUtility.CreateTween(timelineAsset, info.Target, parameter =>
             {
                 if (OverwriteSet == null) return;
                 Set(parameter.Int, OverwriteSet.Ints, info.Parameter);
                 Set(parameter.Float, OverwriteSet.Floats, info.Parameter);
                 Set(parameter.Bool, OverwriteSet.Bools, info.Parameter);
                 Set(parameter.Vector3, OverwriteSet.Vector3s, info.Parameter);
                 Set(parameter.Vector2, OverwriteSet.Vector2s, info.Parameter);
                 Set(parameter.Color, OverwriteSet.Colors, info.Parameter);
             });
         }

         private void Set<TVal, TExp>(TimelineParameterDictionary<TVal> destParamDic, List<ParameterOverwrite<TExp, TVal>> overwrites, 
             TweenParameter parentParameter)
             where TExp : TweenTimelineExpression<TVal>
         {
             if (overwrites == null) return;
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

                if (go == Binding.gameObject && timelineAsset == RootTimelineAsset)
                {
                    Debug.LogWarningFormat("Control Playable ({0}) is referencing the same PlayableDirector component than the one in which it is playing.", name);
                    return Playable.Null;
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
