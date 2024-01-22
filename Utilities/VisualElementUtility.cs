using UnityEngine;
using UnityEngine.UIElements;

namespace TweenTimeline.Editor
{
    public static class VisualElementUtility
    {
        public static Vector2 GetScreenPosition(VisualElement element)
        {
            return GUIUtility.GUIToScreenRect(element.worldBound).position;
        }
    }
}