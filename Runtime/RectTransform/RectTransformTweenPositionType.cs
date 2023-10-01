using System;
using UnityEngine;

namespace TweenTimeline
{
    public enum RectTransformTweenPositionType
    {
        AnchoredPosition = 0, Position = 1, LocalPosition = 2
    }
    
    public static class RectTransformExtensions
    {
        public static Vector3 GetPosition(this RectTransform self, RectTransformTweenPositionType positionType)
        {
            return positionType switch
            {
                RectTransformTweenPositionType.AnchoredPosition => self.anchoredPosition,
                RectTransformTweenPositionType.Position => self.position,
                RectTransformTweenPositionType.LocalPosition => self.localPosition,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static void SetPosition(this RectTransform self, RectTransformTweenPositionType positionType, Vector3 value)
        {
            switch (positionType)
            {
                case RectTransformTweenPositionType.AnchoredPosition:
                    self.anchoredPosition = value;
                    break;
                case RectTransformTweenPositionType.Position:
                    self.position = value;
                    break;
                case RectTransformTweenPositionType.LocalPosition:
                    self.localPosition = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}