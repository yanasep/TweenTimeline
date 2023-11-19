using System;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace TweenTimeline
{
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
        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            using (new EditorGUI.PropertyScope(fieldRect, label, property))
            {
                var easeProp = property.FindPropertyRelative(nameof(EaseOrCurve.Ease));
                var curveProp = property.FindPropertyRelative(nameof(EaseOrCurve.Curve));
                var isCustomProp = property.FindPropertyRelative(nameof(EaseOrCurve.UseAnimationCurve));
                var activeProp = isCustomProp.boolValue ? curveProp : easeProp;
                EditorGUI.PropertyField(fieldRect, activeProp, new GUIContent("Ease"));

                fieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                var rect = EditorGUI.PrefixLabel(fieldRect, new GUIContent(" "));
                var prevIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                EditorGUI.PropertyField(rect, isCustomProp, new GUIContent("Animation Curve"));
                EditorGUI.indentLevel = prevIndent;
            }
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
        }
    }
#endif
}