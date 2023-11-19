using System;
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
    // [TrackClipType(typeof(TweenParameterClip))]
    public class TweenParameterTrack : TrackAsset
    {
        // Unityの不具合？でTrackの最初のfoldoutが表示されないっぽいので適当なフィールドで回避
        [SerializeField, Common.ReadOnly] private byte _;
        
        [Serializable]
        public struct Entry<T>
        {
            public string Name;
            public T Value;
        }
        
        [SerializeField] private Entry<float>[] floats;
        [SerializeField] private Entry<int>[] ints;
        [SerializeField] private Entry<bool>[] bools;
        [SerializeField] private Entry<Vector3>[] vector3s;
        [SerializeField] private Entry<Vector2>[] vector2s;
        [SerializeField] private Entry<Color>[] colors;

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

        private void Set<T>(Entry<T>[] source, TimelineParameterDictionary<T> dest)
        {
            dest.Clear();
            
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
    }
}