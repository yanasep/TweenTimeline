using System;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// EaseかAnimationCurve
    /// </summary>
    [Serializable]
    public struct EaseOrCurve
    {
        public Ease Ease;
        public bool UseAnimationCurve;
        public AnimationCurve Curve;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EaseOrCurve))]
    public class EaseOrCurvePropertyDrawer : PropertyDrawer
    {
        private readonly string[] options = { "Ease", "Curve" };
        private const float PopupWidth = 60;

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using var _ = new EditorGUI.PropertyScope(position, label, property);

            var easeProp = property.FindPropertyRelative(nameof(EaseOrCurve.Ease));
            var curveProp = property.FindPropertyRelative(nameof(EaseOrCurve.Curve));
            var isCurveProp = property.FindPropertyRelative(nameof(EaseOrCurve.UseAnimationCurve));

            var rect = EditorGUI.PrefixLabel(position, label);

            var prevIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var popupPos = rect;
            popupPos.width = PopupWidth;
            var index = isCurveProp.boolValue ? 1 : 0;
            index = EditorGUI.Popup(popupPos, index, options);
            isCurveProp.boolValue = index == 1;
            var selectedProp = isCurveProp.boolValue ? curveProp : easeProp;
            var fieldPos = rect;
            fieldPos.xMin = popupPos.xMax + 2;
            EditorGUI.PropertyField(fieldPos, selectedProp, GUIContent.none);
            EditorGUI.indentLevel = prevIndent;
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
#endif
}
