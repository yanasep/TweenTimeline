using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DG.Tweening;
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
    public class SubTweenClip : TweenClip<PlayableDirector>, IPropertyPreview
    {
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
            public TimelineAsset TimelineAsset;
            public List<ParameterOverwrite<TweenTimelineExpressionInt, int>> Ints;
            public List<ParameterOverwrite<TweenTimelineExpressionFloat, float>> Floats;
            public List<ParameterOverwrite<TweenTimelineExpressionBool, bool>> Bools;
            public List<ParameterOverwrite<TweenTimelineExpressionVector3, Vector3>> Vector3s;
            public List<ParameterOverwrite<TweenTimelineExpressionVector2, Vector2>> Vector2s;
            public List<ParameterOverwrite<TweenTimelineExpressionColor, Color>> Colors;

            internal ParameterOverwrite AddEntry(uint parameterId, Type parameterType)
            {
                var expressionType = GetExpressionType(parameterType);
                var constructedType = typeof(ParameterOverwrite<,>).MakeGenericType(expressionType, parameterType);
                var instance = (ParameterOverwrite)Activator.CreateInstance(constructedType);
                instance.ParameterId = parameterId;
                var expressionField =
                    constructedType.GetField("Expression", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                expressionField?.SetValue(instance, CreateConstantExpression(parameterType));
                var list = GetParameterSetList(parameterType);
                list.Add(instance);
                return instance;
            }

            internal void RemoveEntry(uint parameterId)
            {
                if (TryFindEntry(parameterId, out var list, out var index))
                {
                    list.RemoveAt(index);
                }
            }

            internal ParameterOverwrite GetEntry(uint parameterId)
            {
                if (TryFindEntry(parameterId, out var list, out var index))
                {
                    return (ParameterOverwrite)list[index];
                }

                return null;
            }

            private bool TryFindEntry(uint parameterId, out IList list, out int index)
            {
                if (TryFind(Floats, out index)) { list = Floats; return true; }
                if (TryFind(Ints, out index)) { list = Ints; return true; }
                if (TryFind(Bools, out index)) { list = Bools; return true; }
                if (TryFind(Vector3s, out index)) { list = Vector3s; return true; }
                if (TryFind(Vector2s, out index)) { list = Vector2s; return true; }
                if (TryFind(Colors, out index)) { list = Colors; return true; }
                list = null;
                index = -1;
                return false;

                bool TryFind(IList list, out int index)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var entry = (ParameterOverwrite)list[i];
                        if (entry.ParameterId == parameterId)
                        {
                            index = i;
                            return true;
                        }
                    }

                    index = -1;
                    return false;
                }
            }

            internal (string listPropertyPath, int listIndex) GetPropertyPath(uint parameterId)
            {
                if (!TryFindEntry(parameterId, out var list, out var index))
                {
                    Debug.LogError($"Parameter not found: id={parameterId}");
                    return (null, -1);
                }

                if (list.Equals(Floats)) return (nameof(Floats), index);
                if (list.Equals(Ints)) return (nameof(Ints), index);
                if (list.Equals(Bools)) return (nameof(Bools), index);
                if (list.Equals(Vector3s)) return (nameof(Vector3s), index);
                if (list.Equals(Vector2s)) return (nameof(Vector2s), index);
                if (list.Equals(Colors)) return (nameof(Colors), index);
                return (null, -1);
            }

            private static Type GetExpressionType(Type typeArg)
            {
                Type genericType = typeof(TweenTimelineExpression<>);

                var assembly = typeof(TweenTimelineExpression<>).Assembly;
                foreach (var type in assembly.GetTypes())
                {
                    if (type.BaseType != null && type.BaseType.IsGenericType &&
                        type.BaseType.GetGenericTypeDefinition() == genericType)
                    {
                        var genericArguments = type.BaseType.GetGenericArguments();
                        if (genericArguments.Contains(typeArg))
                        {
                            return type;
                        }
                    }
                }

                return null;
            }

            private static object CreateConstantExpression(Type typeArg)
            {
                var baseType = typeof(TweenTimelineExpression<>).MakeGenericType(typeArg);
                var assembly = typeof(TweenTimelineExpression<>).Assembly;
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsSubclassOf(baseType)) continue;
                    var displayName = type.GetCustomAttribute<DisplayNameAttribute>();
                    if (displayName is { DisplayName: "Constant" })
                    {
                        return Activator.CreateInstance(type);
                    }
                }

                return null;
            }

            private IList GetParameterSetList(Type parameterType)
            {
                return parameterType switch
                {
                    var type when type == typeof(float) => Floats,
                    var type when type == typeof(int) => Ints,
                    var type when type == typeof(bool) => Bools,
                    var type when type == typeof(Vector3) => Vector3s,
                    var type when type == typeof(Vector2) => Vector2s,
                    var type when type == typeof(Color) => Colors,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        [Serializable]
        public abstract class ParameterOverwrite
        {
            public uint ParameterId;
#if UNITY_EDITOR
            /// <summary>インスペクターのリスト表示におけるインデックス</summary>
            public int ViewIndex;
#endif

            internal abstract Type TargetParameterType { get; }
        }

        [Serializable]
        public class ParameterOverwrite<TExpression, TValue> : ParameterOverwrite where TExpression : TweenTimelineExpression<TValue>
        {
            [SerializeReference, SelectableSerializeReference]
            public TExpression Expression;

            internal override Type TargetParameterType => typeof(TValue);
        }

        public override Tween CreateTween(TweenClipInfo<PlayableDirector> info)
        {
            if (OverwriteSet?.TimelineAsset == null) return null;

            return TweenTimelineUtility.CreateTween(OverwriteSet.TimelineAsset, info.Target, parameter =>
            {
                if (OverwriteSet == null) return;
                Set(OverwriteSet.Ints, parameter, info.Parameter);
                Set(OverwriteSet.Floats, parameter, info.Parameter);
                Set(OverwriteSet.Bools, parameter, info.Parameter);
                Set(OverwriteSet.Vector3s, parameter, info.Parameter);
                Set(OverwriteSet.Vector2s, parameter, info.Parameter);
                Set(OverwriteSet.Colors, parameter, info.Parameter);
            });
        }

        private void Set<TVal, TExp>(List<ParameterOverwrite<TExp, TVal>> overwrites, TweenParameter dest, TweenParameter parentParam)
            where TExp : TweenTimelineExpression<TVal>
        {
            if (overwrites == null) return;
            foreach (var overwrite in overwrites)
            {
                if (overwrite.Expression == null) continue;
                dest.SetParameter(overwrite.ParameterId, overwrite.Expression.Evaluate(parentParam));
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

                if (go == Binding.gameObject && OverwriteSet.TimelineAsset == RootTimelineAsset)
                {
                    Debug.LogWarningFormat(
                        "Control Playable ({0}) is referencing the same PlayableDirector component than the one in which it is playing.",
                        name);
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
            PreviewDirector(driver, director, new[] { OverwriteSet.TimelineAsset });
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
