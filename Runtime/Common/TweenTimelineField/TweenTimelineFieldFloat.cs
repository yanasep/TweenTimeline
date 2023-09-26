using System;
using UnityEditor;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    [Serializable]
    public class TweenTimelineField<T>
    {
        public T Value;
    }
    
    [Serializable]
    public class TweenTimelineFieldFloat : TweenTimelineField<float>
    {
    }

    [CustomPropertyDrawer(typeof(TweenTimelineField<>), true)]
    public class TweenTimelineFieldPropertyDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.hasVisibleChildren) return;
            EditorGUI.indentLevel--;

            var fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            using (new EditorGUI.PropertyScope(fieldRect, label, property))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    property.NextVisible(true);
                    var depth = property.depth;
                    // 最初のフィールド名(Valueフィールド)は親のプロパティ名を使う
                    EditorGUI.PropertyField(fieldRect, property, label, true);
                    fieldRect.y += EditorGUI.GetPropertyHeight(property, true);
                    fieldRect.y += EditorGUIUtility.standardVerticalSpacing;

                    // NOTE: foldoutにする？
                    while (property.NextVisible(false))
                    {
                        if (property.depth != depth)
                        {
                            break;
                        }

                        EditorGUI.PropertyField(fieldRect, property, true);
                        fieldRect.y += EditorGUI.GetPropertyHeight(property, true);
                        fieldRect.y += EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }

            EditorGUI.indentLevel++;
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.hasVisibleChildren) return 0;
            
            var height = 0.0f;

            property.NextVisible(true);
            var depth = property.depth;
            height += EditorGUI.GetPropertyHeight(property, true);
            height += EditorGUIUtility.standardVerticalSpacing;

            while (property.NextVisible(false))
            {
                if (property.depth != depth)
                {
                    break;
                }

                height += EditorGUI.GetPropertyHeight(property, true);
                height += EditorGUIUtility.standardVerticalSpacing;
            }

            height -= EditorGUIUtility.standardVerticalSpacing;

            return height;
        }
    }
}