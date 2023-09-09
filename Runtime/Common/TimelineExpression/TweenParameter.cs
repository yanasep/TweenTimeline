using System.Collections.Generic;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// タイムラインに渡すパラメータを保持するクラス
    /// </summary>
    public class TweenParameter
    {   
        public readonly TimelineParameterHolder<float> Float = new();
        public readonly TimelineParameterHolder<int> Int = new();
        public readonly TimelineParameterHolder<bool> Bool = new();
        public readonly TimelineParameterHolder<Vector3> Vector3 = new();
        public readonly TimelineParameterHolder<Vector2> Vector2 = new();
        public readonly TimelineParameterHolder<Color> Color = new();

        /// <summary>文字列をハッシュに</summary>
        public static int StringToHash(string str)
        {
            return str.GetHashCode();
        }
    }

    /// <summary>
    /// 型ごとのパラメータ保持
    /// </summary>
    public class TimelineParameterHolder<T>
    {
        private readonly Dictionary<int, T> valueDic = new();

        /// <summary>パラメータセット</summary>
        public void Set(int keyHash, T value)
        {
            valueDic[keyHash] = value;
        }

        /// <summary>パラメータセット</summary>
        public void Set(string key, T value)
        {
            Set(TweenParameter.StringToHash(key), value);
        }

        /// <summary>パラメータ取得</summary>
        public T GetOrDefault(int keyHash)
        {
            if (valueDic.TryGetValue(keyHash, out var value))
            {
                return value;
            }

            return default;
        }

        /// <summary>パラメータ取得</summary>
        public T GetOrDefault(string key)
        {
            return GetOrDefault(TweenParameter.StringToHash(key));
        }
    }
}
