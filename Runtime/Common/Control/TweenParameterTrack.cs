using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// TweenParameterトラック
    /// </summary>
    [DisplayName("Tween/Tween Parameter Track")]
    // [TrackClipType(typeof(TweenParameterClip))]
    public class TweenParameterTrack : TrackAsset
    {
        [SerializeField] private SerializableDictionary<string, float> floats;
        [SerializeField] private SerializableDictionary<string, int> ints;
        [SerializeField] private SerializableDictionary<string, bool> bools;
        [SerializeField] private SerializableDictionary<string, Vector3> vector3s;
        [SerializeField] private SerializableDictionary<string, Vector2> vector2s;
        [SerializeField] private SerializableDictionary<string, Color> colors;

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

        private void Set<T>(SerializableDictionary<string, T> source, TimelineParameterDictionary<T> dest)
        {
            dest.Clear();
            
            foreach (var (key, val) in source)
            {
                if (string.IsNullOrEmpty(key)) continue;
                dest.Set(key, val);
            }
        }
        
        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return default;
        }
    }
}