using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// TweenTimelineのパラメータのデフォルト値を保持するコンポーネント
    /// </summary>
    public class TweenParameterHolder : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<string, float> floats;
        [SerializeField] private SerializableDictionary<string, int> ints;
        [SerializeField] private SerializableDictionary<string, bool> bools;
        [SerializeField] private SerializableDictionary<string, Vector3> vector3s;
        [SerializeField] private SerializableDictionary<string, Vector2> vector2s;
        [SerializeField] private SerializableDictionary<string, Color> colors;

        /// <summary>
        /// デフォルト値が入ったTimelineParameterContainerを生成して取得
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
            foreach (var (key, val) in source)
            {
                if (string.IsNullOrEmpty(key)) continue;
                dest.Set(key, val);
            }
        }
    }
}
