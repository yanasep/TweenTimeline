using System;
using UnityEditor;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    public interface ITweenTimelineField
    {
        void SetValue(TweenParameter parameter);

        ITweenTimelineField Clone();
    }

    public enum TweenTimelineFieldType
    {
        Constant,
        Parameter
    }

    [Serializable]
    public class TweenTimelineField<T> : ITweenTimelineField
    {
        public T ConstantValue;
        public string ParameterName;
        public TweenTimelineFieldType FieldType;
        
        public T Value { get; private set; }
        
        public TweenTimelineField(T value)
        {
            ConstantValue = value;
        }

        public void SetValue(TweenParameter parameter)
        {
            switch (FieldType)
            {
                case TweenTimelineFieldType.Constant:
                    Value = ConstantValue;
                    break;
                case TweenTimelineFieldType.Parameter:
                    // Value = parameter.
                    break;
            }
        }

        public ITweenTimelineField Clone()
        {
            return (ITweenTimelineField)MemberwiseClone();
        }
    }

    [Serializable]
    public abstract class TweenTimelineFieldExpression<T> : ITweenTimelineField
    {
        public abstract TweenTimelineExpression<T> Expression { get; }
        public T Value { get; private set; }

        public void SetValue(TweenParameter parameter)
        {
            Value = Expression.Evaluate(parameter);
        }

        public ITweenTimelineField Clone()
        {
            // NOTE: expressionは使いまわし可能なのでクローンしくてOK
            return (ITweenTimelineField)MemberwiseClone();
        }
    }

    [Serializable]
    public class TweenTimelineFieldInt : TweenTimelineFieldExpression<int>
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionInt expression;

        public override TweenTimelineExpression<int> Expression => expression;
        
        public TweenTimelineFieldInt(int value)
        {
            expression = new TweenTimelineExpressionIntConstant { Value = value };
        }
    }

    [Serializable]
    public class TweenTimelineFieldFloat : TweenTimelineFieldExpression<float>
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat expression;

        public override TweenTimelineExpression<float> Expression => expression;
        
        public TweenTimelineFieldFloat(float value)
        {
            expression = new TweenTimelineExpressionFloatConstant { Value = value };
        }
    }

    [Serializable]
    public class TweenTimelineFieldBool : TweenTimelineFieldExpression<bool>
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionBool expression;

        public override TweenTimelineExpression<bool> Expression => expression;
        
        public TweenTimelineFieldBool(bool value)
        {
            expression = new TweenTimelineExpressionBoolConstant { Value = value };
        }
    }

    [Serializable]
    public class TweenTimelineFieldColor : TweenTimelineFieldExpression<Color>
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionColor expression;

        public override TweenTimelineExpression<Color> Expression => expression;
        
        public TweenTimelineFieldColor(Color value)
        {
            expression = new TweenTimelineExpressionColorConstant { Value = value };
        }
    }

    [Serializable]
    public class TweenTimelineFieldVector3 : TweenTimelineFieldExpression<Vector3>
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 expression;

        public override TweenTimelineExpression<Vector3> Expression => expression;
        
        public TweenTimelineFieldVector3(Vector3 value)
        {
            expression = new TweenTimelineExpressionVector3Constant { Value = value };
        }
    }

    [Serializable]
    public class TweenTimelineFieldVector2 : TweenTimelineFieldExpression<Vector2>
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector2 expression;

        public override TweenTimelineExpression<Vector2> Expression => expression;
        
        public TweenTimelineFieldVector2(Vector2 value)
        {
            expression = new TweenTimelineExpressionVector2Constant { Value = value };
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TweenTimelineField<>))]
    public sealed class TweenTimelineFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            
            label = EditorGUI.BeginProperty(position, label, property);

            // label
            var labelPos = position;
            labelPos.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(labelPos, label);
            
            // field type
            var fieldTypeProperty = property.FindPropertyRelative(nameof(TweenTimelineField<int>.FieldType));
            var fieldType = (TweenTimelineFieldType)fieldTypeProperty.intValue;
             
            var fieldTypePos = position;
            fieldTypePos.xMin = labelPos.xMax;
            fieldTypePos.width = 80;

            using (var change = new EditorGUI.ChangeCheckScope())
            {
                fieldType = (TweenTimelineFieldType)EditorGUI.EnumPopup(fieldTypePos, fieldType);
                if (change.changed)
                {
                    fieldTypeProperty.intValue = (int)fieldType;
                    fieldTypeProperty.serializedObject.ApplyModifiedProperties();
                }
            }

            // field
            var fieldProperty = fieldType switch
            {
                TweenTimelineFieldType.Constant => property.FindPropertyRelative(nameof(TweenTimelineField<int>.ConstantValue)),
                TweenTimelineFieldType.Parameter => property.FindPropertyRelative(nameof(TweenTimelineField<int>.ParameterName)),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            var fieldPos = position;
            fieldPos.xMin = fieldTypePos.xMax + 5;
            EditorGUI.PropertyField(fieldPos, fieldProperty, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
    
    [CustomPropertyDrawer(typeof(TweenTimelineFieldExpression<>), useForChildren: true)]
    public sealed class TweenTimelineFieldExpressionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var fieldProperty = property.FindPropertyRelative(nameof(TweenTimelineFieldInt.expression));
            EditorGUI.PropertyField(position, fieldProperty, label);
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var fieldProperty = property.FindPropertyRelative(nameof(TweenTimelineFieldInt.expression));
            return EditorGUI.GetPropertyHeight(fieldProperty);
        }
    }
#endif
}