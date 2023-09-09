using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// TweenTimelineのパラメータのデフォルト値を保持するコンポーネント
    /// </summary>
    public class TweenTimelineDefaultParameter : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<string, float> floats;
        [SerializeField] private SerializableDictionary<string, Vector3> vector3s;
        [SerializeField] private SerializableDictionary<string, bool> bools;

        /// <summary>
        /// デフォルト値が入ったTimelineParameterContainerを生成して取得
        /// </summary>
        public TimelineParameterContainer GetParameterContainer()
        {
            var parameter = new TimelineParameterContainer();
            
            foreach (var (key, val) in floats)
            {
                if (string.IsNullOrEmpty(key)) continue;
                parameter.Float.Set(key, val);
            }

            foreach (var (key, val) in vector3s)
            {
                if (string.IsNullOrEmpty(key)) continue;
                parameter.Vector3.Set(key, val);
            }

            foreach (var (key, val) in bools)
            {
                if (string.IsNullOrEmpty(key)) continue;
                parameter.Bool.Set(key, val);
            }

            return parameter;
        }
    }
}
