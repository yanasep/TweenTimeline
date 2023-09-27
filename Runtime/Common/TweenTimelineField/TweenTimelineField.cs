using System;
using UnityEditor;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// TweenTimelineで値のオーバーライドが可能なフィールド
    /// </summary>
    public abstract class TweenTimelineField
    {
    }

    /// <summary>
    /// TweenTimelineで値のオーバーライドが可能なフィールド
    /// </summary>
    [Serializable]
    public sealed class TweenTimelineField<T> : TweenTimelineField, ISerializationCallbackReceiver
    {
        /// <summary>SerializeFieldで直接設定された値</summary>
        [SerializeField] private T predefinedValue;
        public T PredefinedValue { get => predefinedValue; set => predefinedValue = value; }
        
        /// <summary>上書きされた値</summary>
        public T Value { get; set; }

        /// <summary>コンストラクタ</summary>
        public TweenTimelineField()
        {
        }
        
        /// <summary>コンストラクタ</summary>
        public TweenTimelineField(T value)
        {
            PredefinedValue = value;
            Value = value;
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
            Value = PredefinedValue;
        }
    }

    /// <summary>
    /// TweenTimelineFieldのPropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(TweenTimelineField<>), true)]
    public class TweenTimelineFieldPropertyDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // foldoutなし、ラベルなしでPredefinedValueのみを描画
            EditorGUI.PropertyField(position, property.FindPropertyRelative("predefinedValue"), label);
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}