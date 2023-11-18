// using UnityEngine;
// using Yanasep;
//
// namespace TweenTimeline
// {
//     /// <summary>
//     /// TweenTimelineのパラメータのデフォルト値を保持するコンポーネント
//     /// </summary>
//     public class TweenParameterHolder : MonoBehaviour
//     {
//         [SerializeField] private SerializableDictionary<string, float> floats;
//         [SerializeField] private SerializableDictionary<string, int> ints;
//         [SerializeField] private SerializableDictionary<string, bool> bools;
//         [SerializeField] private SerializableDictionary<string, Vector3> vector3s;
//         [SerializeField] private SerializableDictionary<string, Vector2> vector2s;
//         [SerializeField] private SerializableDictionary<string, Color> colors;
//
//         private TweenParameter _parameter;
//
//         /// <summary>
//         /// TimelineParameterContainerを取得
//         /// </summary>
//         public TweenParameter GetParameter()
//         {
//             _parameter ??= new TweenParameter();
//             Set(floats, _parameter.Float);
//             Set(ints, _parameter.Int);
//             Set(bools, _parameter.Bool);
//             Set(vector3s, _parameter.Vector3);
//             Set(vector2s, _parameter.Vector2);
//             Set(colors, _parameter.Color);
//
//             return _parameter;
//         }
//
//         private void Set<T>(SerializableDictionary<string, T> source, TimelineParameterDictionary<T> dest)
//         {
//             dest.Clear();
//             
//             foreach (var (key, val) in source)
//             {
//                 if (string.IsNullOrEmpty(key)) continue;
//                 dest.Set(key, val);
//             }
//         }
//     }
// }
