using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TweenTimeline
{
    public static class SubTweenClipEditorUtility
    {
        public static IReadOnlyList<SubTweenClip.ParameterOverwrite> GetParameterSetEntries(SubTweenClip clip, TweenParameterType type)
        {
            return type switch
            {
                TweenParameterType.Int => clip.OverwriteSet.Ints,
                TweenParameterType.Float => clip.OverwriteSet.Floats,
                TweenParameterType.Bool => clip.OverwriteSet.Bools,
                TweenParameterType.Vector3 => clip.OverwriteSet.Vector3s,
                TweenParameterType.Vector2 => clip.OverwriteSet.Vector2s,
                TweenParameterType.Color => clip.OverwriteSet.Colors,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public static IList GetParameterSetEntriesAsList(SubTweenClip clip, TweenParameterType type)
        {
            return (IList)GetParameterSetEntries(clip, type);
        }

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
