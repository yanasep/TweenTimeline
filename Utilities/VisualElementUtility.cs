using UnityEngine;
using UnityEngine.UIElements;

namespace TweenTimeline
{
    /// <summary>
    /// UIElementsのUtility・拡張
    /// </summary>
    public static class VisualElementUtility
    {
        /// <summary>
        /// 表示・非表示
        /// </summary>
        public static void SetVisible(this VisualElement element, bool visible)
        {
            element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>
        /// VisualElementの位置をScreen座標で取得
        /// </summary>
        public static Vector2 GetScreenPosition(this VisualElement element)
        {
            return GUIUtility.GUIToScreenRect(element.worldBound).position;
        }

        /// <summary>
        /// ListViewで選択されている要素を取得
        /// </summary>
        public static VisualElement GetSelectedItem(this ListView listView)
        {
            return listView.Q(className: "unity-collection-view__item--selected");
        }
    }
}
