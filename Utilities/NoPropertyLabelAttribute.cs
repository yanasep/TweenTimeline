using System;
using UnityEditor;
using UnityEngine;

namespace Yanasep
{
    /// <summary>
    /// プロパティ名なしで描画する
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NoPropertyLabelAttribute : MultiPropertyAttribute, IMultiPropertyGUIAttribute
    {
#if UNITY_EDITOR
        /// <inheritdoc/>
        public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, GUIContent.none, true);
        }

        /// <inheritdoc/>
        public float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.type is "Vector3" or "Vector2")
            {
                return EditorGUIUtility.singleLineHeight;
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
#endif
    }
}