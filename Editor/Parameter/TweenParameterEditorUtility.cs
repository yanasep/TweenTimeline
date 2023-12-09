using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TweenTimeline
{
    public static class TweenParameterEditorUtility
    {
        public static IReadOnlyList<TweenParameterTrack.ParameterSetEntry> GetParameterSetEntries(TweenParameterTrack track, TweenParameterType type)
        {
            return type switch
            {
                TweenParameterType.Int => track.ints ??= new(),
                TweenParameterType.Float => track.floats ??= new(),
                TweenParameterType.Bool => track.bools ??= new(),
                TweenParameterType.Vector3 => track.vector3s ??= new(),
                TweenParameterType.Vector2 => track.vector2s ??= new(),
                TweenParameterType.Color => track.colors ??= new(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public static IList GetParameterSetEntriesAsList(TweenParameterTrack track, TweenParameterType type)
        {
            return (IList)GetParameterSetEntries(track, type);
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
