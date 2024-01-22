using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TweenTimeline
{
    /// <summary>
    /// 他のフィールドの値を条件として有効状態を切り替える
    /// </summary>
    /// <remarks>https://light11.hatenadiary.com/entry/2019/03/24/013917</remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EnableIfAttribute : MultiPropertyAttribute
    {
        /// <summary>
        /// 非表示状態の表現方法
        /// </summary>
        public enum HideMode
        {
            Invisible, // 見えなくする
            Disabled, // 非アクティブにする
        }

        /// <summary>変更したい場合は名前付きパラメータで EnableIf(..., ..., hideMode = HideMode.Disabled)</summary>
        public HideMode hideMode { get; set; } = HideMode.Invisible;

        public readonly string sourceFieldName;
        public readonly bool targetBoolValue;
        public readonly int[] targetEnumValues;

        public EnableIfAttribute(string sourceFieldName, bool targetBoolValue)
            : this(sourceFieldName)
        {
            this.targetBoolValue = targetBoolValue;
        }

        public EnableIfAttribute(string sourceFieldName, params int[] targetEnumValues)
            : this(sourceFieldName)
        {
            this.targetEnumValues = targetEnumValues;
        }

        private EnableIfAttribute(string sourceFieldName)
        {
            this.sourceFieldName = sourceFieldName;
        }

#if UNITY_EDITOR
        public override void OnPreGUI(Rect position, SerializedProperty property)
        {
            var isEnabled = GetIsEnabled(property);

            if (hideMode == HideMode.Disabled)
            {
                GUI.enabled &= isEnabled;
            }
        }

        public override void OnPostGUI(Rect position, SerializedProperty property, bool changed)
        {
            if (hideMode == HideMode.Disabled)
            {
                GUI.enabled = true;
            }
        }

        public override bool IsVisible(SerializedProperty property)
        {
            return hideMode != HideMode.Invisible || GetIsEnabled(property);
        }

        /// <summary>
        /// 有効化条件を満たしているか
        /// </summary>
        private bool GetIsEnabled(SerializedProperty property)
        {
            var propertyNameIndex = property.propertyPath.LastIndexOf(property.name, StringComparison.Ordinal);
            var sourcePropertyName = property.propertyPath.Substring(0, propertyNameIndex) + sourceFieldName;
            var sourceProperty = property.serializedObject.FindProperty(sourcePropertyName);
            switch (sourceProperty.propertyType)
            {
                case SerializedPropertyType.Boolean:
                {
                    return sourceProperty.boolValue == targetBoolValue;
                }
                case SerializedPropertyType.Enum:
                {
                    return targetEnumValues.Contains(sourceProperty.intValue);
                }
                default:
                    Debug.LogError($"[EnableIfAttribute] Unsupported type. {property.propertyType}");
                    return false;
            }
        }
#endif
    }
}
