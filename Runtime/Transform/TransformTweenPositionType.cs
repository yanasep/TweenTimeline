using System;
using UnityEngine;

namespace TweenTimeline
{
    public enum TransformTweenPositionType
    {
        Position = 0,
        LocalPosition = 1
    }

    public static class TransformExtensions
    {
        public static Vector3 GetPosition(this Transform self, TransformTweenPositionType positionType)
        {
            return positionType switch
            {
                TransformTweenPositionType.Position => self.position,
                TransformTweenPositionType.LocalPosition => self.localPosition,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static void SetPosition(this Transform self, TransformTweenPositionType positionType, Vector3 value)
        {
            switch (positionType)
            {
                case TransformTweenPositionType.Position:
                    self.position = value;
                    break;
                case TransformTweenPositionType.LocalPosition:
                    self.localPosition = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}