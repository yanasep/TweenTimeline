using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// TweenParameterトラック
    /// </summary>
    [DisplayName("Tween/Tween Parameter Track")]
    public class TweenParameterTrack : TrackAsset
    {
        // Unityの不具合？でTrackの最初のfoldoutが表示されないっぽいので適当なフィールドで回避
        [SerializeField, Common.ReadOnly] private byte _;
        
        [Serializable]
        public class ParameterSetEntry
        {
            public string Name;
            /// <summary>インスペクターのリスト表示におけるインデックス</summary>
            public int ViewIndex;   
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

        /// <summary>
        /// TimelineParameterContainerを取得
        /// </summary>
        public TweenParameter GetParameter()
        {
            var parameter = new TweenParameter();
            Set(floats, parameter.Float);
            Set(ints, parameter.Int);
            Set(bools, parameter.Bool);
            Set(vector3s, parameter.Vector3);
            Set(vector2s, parameter.Vector2);
            Set(colors, parameter.Color);

            return parameter;
        }

        private void Set<T>(List<ParameterSetEntry<T>> source, TimelineParameterDictionary<T> dest)
        {
            dest.Clear();

            if (source == null) return;
            foreach (var entry in source)
            {
                if (string.IsNullOrEmpty(entry.Name)) continue;
                dest.Set(entry.Name, entry.Value);
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