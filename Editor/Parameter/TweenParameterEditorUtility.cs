using System;
using UnityEngine;

namespace TweenTimeline.Editor
{
    /// <summary>
    /// TweenParameterのエディタ用Utility
    /// </summary>
    public static class TweenParameterEditorUtility
    {
        /// <summary>
        /// TweenParameterTypeをTypeに変換する
        /// </summary>
        public static Type ParameterTypeToType(TweenParameterType type)
        {
            return type switch
            {
                TweenParameterType.Int => typeof(int),
                TweenParameterType.Float => typeof(float),
                TweenParameterType.Bool => typeof(bool),
                TweenParameterType.Vector3 => typeof(Vector3),
                TweenParameterType.Vector2 => typeof(Vector2),
                TweenParameterType.Color => typeof(Color),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        /// <summary>
        /// TypeをTweenParameterTypeに変換する
        /// </summary>
        public static TweenParameterType TypeToParameterType(Type type)
        {
            if (type == typeof(int)) return TweenParameterType.Int;
            if (type == typeof(float)) return TweenParameterType.Float;
            if (type == typeof(bool)) return TweenParameterType.Bool;
            if (type == typeof(Vector3)) return TweenParameterType.Vector3;
            if (type == typeof(Vector2)) return TweenParameterType.Vector2;
            if (type == typeof(Color)) return TweenParameterType.Color;
            throw new ArgumentException($"Unsupported type: {type}", nameof(type));
        }
    }
}
