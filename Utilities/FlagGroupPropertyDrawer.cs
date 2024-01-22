using UnityEditor;
using UnityEngine;

namespace Yanasep
{
    /// <summary>
    /// 複数のフラグをまとめて管理するクラスのベース
    /// </summary>
    public class FlagGroup
    {
    }

#if UNITY_EDITOR
    /// <summary>
    /// FlagGroupのPropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(FlagGroup), true)]
    public class FlagGroupPropertyDrawer : PropertyDrawer
    {
        private const float LabelWidth = 12;
        private const float ValueWidth = 16;
        private const float Spacing = 6;

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 一行で描画する

            // 名前
            label = EditorGUI.BeginProperty(position, label, property);
            Rect fieldPos = EditorGUI.PrefixLabel(position, label);

            if (!property.hasVisibleChildren) return;

            var prevIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // 各要素を描画
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = LabelWidth;

            var flagPos = fieldPos;

            property.NextVisible(true);
            var depth = property.depth;

            flagPos.width = LabelWidth + ValueWidth;
            do
            {
                if (property.depth != depth) break;

                EditorGUI.PropertyField(flagPos, property, true);
                flagPos.x += flagPos.width + Spacing;
            } while (property.NextVisible(false));

            EditorGUIUtility.labelWidth = prevLabelWidth;

            EditorGUI.indentLevel = prevIndent;

            EditorGUI.EndProperty();
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
#endif
}
