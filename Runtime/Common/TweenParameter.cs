using System.Collections.Generic;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// タイムラインに渡すパラメータを保持するクラス
    /// </summary>
    public class TweenParameter
    {   
        public readonly TimelineParameterDictionary<float> Float = new();
        public readonly TimelineParameterDictionary<int> Int = new();
        public readonly TimelineParameterDictionary<bool> Bool = new();
        public readonly TimelineParameterDictionary<Vector3> Vector3 = new();
        public readonly TimelineParameterDictionary<Vector2> Vector2 = new();
        public readonly TimelineParameterDictionary<Color> Color = new();

        /// <summary>文字列をハッシュに</summary>
        public static int StringToHash(string str)
        {
            return str.GetHashCode();
        }

        public void OverwriteFrom(TweenParameter source)
        {
            Float.OverwriteFrom(source.Float);
            Int.OverwriteFrom(source.Int);
            Bool.OverwriteFrom(source.Bool);
            Vector3.OverwriteFrom(source.Vector3);
            Vector2.OverwriteFrom(source.Vector2);
            Color.OverwriteFrom(source.Color);
        }

        public TweenParameter Clone()
        {
            var clone = new TweenParameter();
            clone.OverwriteFrom(this);
            return clone;
        }
    }

    /// <summary>
    /// 型ごとのパラメータ保持
    /// </summary>
    public class TimelineParameterDictionary<T>
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

        public void Clear()
        {
            valueDic.Clear();
        }

        public void OverwriteFrom(TimelineParameterDictionary<T> source)
        {
            if (source == this)
            {
                Debug.LogWarning($"Trying to overwrite the same instance");
                return;
            }
            
            foreach (var (key, val) in source.valueDic)
            {
                valueDic[key] = val;
            }
        }
    }
}
