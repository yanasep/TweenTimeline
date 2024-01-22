using System;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
#endif

namespace TweenTimeline
{
    /// <summary>
    /// SerializeReferenceのドロップダウン
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SelectableSerializeReferenceAttribute : MultiPropertyAttribute, IMultiPropertyGUIAttribute
    {
#if UNITY_EDITOR
        private const float SingleLinePropertyWidth = 100;

        private readonly Dictionary<string, PropertyData> dataPerPath = new();

        private PropertyData data;

        private int selectedIndex;

        /// <summary>
        /// 初期化
        /// </summary>
        private void Init(SerializedProperty property)
        {
            if (dataPerPath.TryGetValue(property.propertyPath, out data))
            {
                return;
            }

            data = new PropertyData(property);
            dataPerPath.Add(property.propertyPath, data);
        }

        /// <summary>
        /// GUIの描写
        /// </summary>
        public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Assert.IsTrue(property.propertyType == SerializedPropertyType.ManagedReference);

            Init(property);

            var valueRect = EditorGUI.PrefixLabel(position, label);

            var fullTypeName = property.managedReferenceFullTypename.Split(' ').Last();
            // インナークラスの区切り文字をType型に合わせる
            fullTypeName = fullTypeName.Replace('/', '+');
            selectedIndex = Array.IndexOf(data.DerivedFullTypeNames, fullTypeName);

            var selectorPosition = valueRect;

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            selectorPosition.height = EditorGUIUtility.singleLineHeight;

            var singleLineAttribute = selectedIndex >= 0 ? data.SingleLineAttributes[selectedIndex] : null;
            if (singleLineAttribute != null)
            {
                selectorPosition.width = SingleLinePropertyWidth;
            }
            else if (string.IsNullOrEmpty(label.text))
            {
                selectorPosition.xMin += 15;
            }

            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var selectedTypeIndex = EditorGUI.Popup(selectorPosition, selectedIndex, data.DerivedTypeNames);
                if (ccs.changed)
                {
                    selectedIndex = selectedTypeIndex;
                    var selectedType = data.DerivedTypes[selectedTypeIndex];
                    property.managedReferenceValue = selectedType == null ? null : Activator.CreateInstance(selectedType);
                    EditorGUI.indentLevel = indent;
                    return;
                }
            }

            if (singleLineAttribute != null)
            {
                var propertyPos = position;
                propertyPos.xMin = selectorPosition.xMax;
                propertyPos.width /= singleLineAttribute.ParamNames.Length;
                foreach (var paramName in singleLineAttribute.ParamNames)
                {
                    var child = property.FindPropertyRelative(paramName);
                    // ラベル上ドラッグでの値変更ができるようにするため、空のラベルを描画
                    var originalLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 3;
                    EditorGUI.PropertyField(propertyPos, child, new GUIContent(" "), false);
                    EditorGUIUtility.labelWidth = originalLabelWidth;
                    propertyPos.x += propertyPos.width;
                }
                EditorGUI.indentLevel = indent;
                return;
            }
            EditorGUI.indentLevel = indent;

            // ラベル無しで描画
            // NOTE: MultiPropertyAttributeが複数ついている場合にこのクラスのOnGUIが多重に呼び出される。位置合わせのため空文字にしない。
            EditorGUI.PropertyField(position, property, new GUIContent(" "), true);
        }

        /// <summary>
        /// プロパティの高さを取得
        /// </summary>
        public float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property);

            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                return EditorGUIUtility.singleLineHeight;
            }
            if (selectedIndex >= 0 && data.SingleLineAttributes[selectedIndex] != null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUI.GetPropertyHeight(property, true);
        }

        /// <summary>
        /// プロパティのデータクラス
        /// </summary>
        private class PropertyData
        {
            public PropertyData(SerializedProperty property)
            {
                var managedReferenceFieldTypenameSplit = property.managedReferenceFieldTypename.Split(' ').ToArray();
                var assemblyName = managedReferenceFieldTypenameSplit[0];
                var fieldTypeName = managedReferenceFieldTypenameSplit[1];
                var fieldType = GetAssembly(assemblyName).GetType(fieldTypeName);
                DerivedTypes = TypeCache.GetTypesDerivedFrom(fieldType).Where(x => !x.IsAbstract && !x.IsInterface)
                    .ToArray();
                DerivedTypeNames = new string[DerivedTypes.Length];
                DerivedFullTypeNames = new string[DerivedTypes.Length];
                SingleLineAttributes = new SelectableSerializeSingleLineAttribute[DerivedTypes.Length];
                for (var i = 0; i < DerivedTypes.Length; i++)
                {
                    var type = DerivedTypes[i];
                    string displayName = type.Name;
                    SelectableSerializeSingleLineAttribute singleLine = null;
                    foreach (var attr in type.GetCustomAttributes())
                    {
                        if (attr is System.ComponentModel.DisplayNameAttribute displayNameAttr)
                        {
                            displayName = displayNameAttr.DisplayName;
                        }
                        singleLine ??= attr as SelectableSerializeSingleLineAttribute;
                    }
                    DerivedTypeNames[i] = ObjectNames.NicifyVariableName(displayName);
                    DerivedFullTypeNames[i] = type.FullName;
                    SingleLineAttributes[i] = singleLine;
                }
            }

            public Type[] DerivedTypes { get; }

            public string[] DerivedTypeNames { get; }

            public string[] DerivedFullTypeNames { get; }

            public SelectableSerializeSingleLineAttribute[] SingleLineAttributes { get; }

            /// <summary>
            /// Assemblyの取得
            /// </summary>
            private static Assembly GetAssembly(string name)
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SingleOrDefault(assembly => assembly.GetName().Name == name);
            }
        }
#endif
    }

    /// <summary>
    /// １行で描画する設定
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class SelectableSerializeSingleLineAttribute : Attribute
    {
        public readonly string[] ParamNames;

        public SelectableSerializeSingleLineAttribute(params string[] paramNames)
        {
            ParamNames = paramNames;
        }
    }
}
