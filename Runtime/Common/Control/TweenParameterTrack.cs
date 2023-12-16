﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

namespace TweenTimeline
{
    /// <summary>
    /// TweenParameterトラック
    /// </summary>
    [DisplayName("Tween/Tween Parameter Track")]
    public class TweenParameterTrack : TrackAsset
    {
        [Serializable]
        public class ParameterSetEntry
        {
            public uint ParameterId;
            public string ParameterName;
#if UNITY_EDITOR
            /// <summary>インスペクターのリスト表示におけるインデックス</summary>
            public int ViewIndex;
#endif
        }
        
        [Serializable]
        public class ParameterSetEntry<T> : ParameterSetEntry
        {
            public T Value;
        }
        
        public List<ParameterSetEntry<float>> floats;
        public List<ParameterSetEntry<int>> ints;
        public List<ParameterSetEntry<bool>> bools;
        public List<ParameterSetEntry<Vector3>> vector3s;
        public List<ParameterSetEntry<Vector2>> vector2s;
        public List<ParameterSetEntry<Color>> colors;

        private uint GenerateParameterId()
        {
            uint result;
            do
            {
                result = RandomUint();
            } while (GetAllEntries().Any(x => x.ParameterId == result));

            return result;
        }

        private uint RandomUint()
        {
            uint thirtyBits = (uint) Random.Range(0, 1 << 30);
            uint twoBits = (uint) Random.Range(0, 1 << 2);
            return (thirtyBits << 2) | twoBits;
        }

        internal ParameterSetEntry AddEntry(string parameterName, Type parameterType)
        {
            var entry = CreateEntryOfType(parameterType);
            entry.ParameterName = parameterName;
            entry.ParameterId = GenerateParameterId();
            ParameterSetEntryConverter.SetDefaultValue(entry);
            var list = GetParameterSetList(parameterType);
            list.Add(entry);
            return entry;
        }

        private ParameterSetEntry CreateEntryOfType(Type parameterType)
        {
            var constructedType = typeof(ParameterSetEntry<>).MakeGenericType(parameterType);
            var instance = (ParameterSetEntry)Activator.CreateInstance(constructedType);
            return instance;
        }

        internal void RemoveEntry(uint parameterId)
        {
            if (TryFindEntry(parameterId, out var list, out var index))
            {
                list.RemoveAt(index);
            }
        }

        internal ParameterSetEntry GetEntry(uint parameterId)
        {
            if (TryFindEntry(parameterId, out var list, out var index))
            {
                return (ParameterSetEntry)list[index];
            }

            return null;
        }
        
        private bool TryFindEntry(uint parameterId, out IList list, out int index)
        {
            if (TryFind(floats, out index)) { list = floats; return true; }
            if (TryFind(ints, out index)) { list = ints; return true; }
            if (TryFind(bools, out index)) { list = bools; return true; }
            if (TryFind(vector3s, out index)) { list = vector3s; return true; }
            if (TryFind(vector2s, out index)) { list = vector2s; return true; }
            if (TryFind(colors, out index)) { list = colors; return true; }
            list = null;
            index = -1;
            return false;
            
            bool TryFind(IList list, out int index)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var entry = (ParameterSetEntry)list[i];
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
        
        internal ParameterSetEntry ConvertEntryType(uint parameterId, Type newType)
        {
            if (!TryFindEntry(parameterId, out var prevList, out var prevListIndex))
            {
                Debug.LogError($"Parameter not found: id={parameterId}");
                return null;
            }

            var newList = GetParameterSetList(newType);
            if (newList.Equals(prevList))
            {
                Debug.LogError($"Conversion to same type is not allowed: id={parameterId} type={newType}");
                return null;
            }

            var prevEntry = (ParameterSetEntry)prevList[prevListIndex];
            prevList.RemoveAt(prevListIndex);
            
            var newEntry = CreateEntryOfType(newType);
            newEntry.ParameterId = parameterId;
            newEntry.ParameterName = prevEntry.ParameterName;
            ParameterSetEntryConverter.SetDefaultValue(newEntry);
            ParameterSetEntryConverter.TryConvert(prevData: prevEntry, newData: newEntry);
            newList.Add(newEntry);
            return newEntry;
        }

        internal (string listPropertyPath, int listIndex) GetPropertyPath(uint parameterId)
        {
            if (!TryFindEntry(parameterId, out var list, out var index))
            {
                Debug.LogError($"Parameter not found: id={parameterId}");
                return (null, -1);
            }
            
            if (list.Equals(floats)) return (nameof(floats), index);
            if (list.Equals(ints)) return (nameof(ints), index);
            if (list.Equals(bools)) return (nameof(bools), index);
            if (list.Equals(vector3s)) return (nameof(vector3s), index);
            if (list.Equals(vector2s)) return (nameof(vector2s), index);
            if (list.Equals(colors)) return (nameof(colors), index);
            return (null, -1);
        }

        internal Type GetParameterType(ParameterSetEntry entry)
        {
            var entryType = entry.GetType();
            return entryType.GetGenericArguments()[0];
        }

        private IList GetParameterSetList(Type parameterType)
        {
            return parameterType switch
            {
                var type when type == typeof(float) => floats,
                var type when type == typeof(int) => ints,
                var type when type == typeof(bool) => bools,
                var type when type == typeof(Vector3) => vector3s,
                var type when type == typeof(Vector2) => vector2s,
                var type when type == typeof(Color) => colors,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        internal IEnumerable<ParameterSetEntry> GetEntriesOfType(Type parameterType)
        {
            var list = GetParameterSetList(parameterType);
            return list.Cast<ParameterSetEntry>();
        }
        
        private IEnumerable<ParameterSetEntry> GetAllEntries()
        {
            return floats.Cast<ParameterSetEntry>().Concat(ints).Concat(bools).Concat(vector3s).Concat(vector2s).Concat(colors);
        }

        /// <summary>
        /// TimelineParameterContainerを取得
        /// </summary>
        public TweenParameter GetParameter()
        {
            var parameter = new TweenParameter();
            Add(floats, parameter);
            Add(ints, parameter);
            Add(bools, parameter);
            Add(vector3s, parameter);
            Add(vector2s, parameter);
            Add(colors, parameter);

            return parameter;
        }

        private void Add<T>(List<ParameterSetEntry<T>> source, TweenParameter dest)
        {
            if (source == null) return;
            foreach (var entry in source)
            {
                if (string.IsNullOrEmpty(entry.ParameterName)) continue;
                dest.AddParameter(entry.ParameterId, entry.ParameterName, entry.Value);
            }
        }
        
        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return default;
        }
        
#if UNITY_EDITOR
        private static readonly HashSet<PlayableDirector> s_ProcessedDirectors = new HashSet<PlayableDirector>();
        
        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            if (director == null)
                return;

            // avoid recursion
            if (s_ProcessedDirectors.Contains(director))
                return;

            s_ProcessedDirectors.Add(director);

            var subDirectorsToPreview = new HashSet<PlayableDirector>();

            foreach (var clip in GetClips())
            {
                var controlPlayableAsset = clip.asset as ControlPlayableAsset;
                if (controlPlayableAsset == null)
                    continue;

                var gameObject = controlPlayableAsset.sourceGameObject.Resolve(director);
                if (gameObject == null)
                    continue;
                
                subDirectorsToPreview.UnionWith(GetComponent<PlayableDirector>(gameObject));
            }

            PreviewDirectors(driver, subDirectorsToPreview);

            s_ProcessedDirectors.Remove(director);

            subDirectorsToPreview.Clear();
        }
        
        internal static void PreviewDirectors(IPropertyCollector driver, IEnumerable<PlayableDirector> directors)
        {
            foreach (var childDirector in directors)
            {
                if (childDirector == null)
                    continue;

                var timeline = childDirector.playableAsset as TimelineAsset;
                if (timeline == null)
                    continue;

                timeline.GatherProperties(childDirector, driver);
            }
        }
        
        internal IList<T> GetComponent<T>(GameObject gameObject)
        {
            var components = new List<T>();
            if (gameObject != null)
            {
                gameObject.GetComponents<T>(components);
            }
            return components;
        }
#endif
    }
}