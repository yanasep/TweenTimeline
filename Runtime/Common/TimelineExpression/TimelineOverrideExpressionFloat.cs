using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// floatの値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineOverrideExpressionFloat : TimelineOverrideExpression<float>
    {
    }

    /// <summary>
    /// floatの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("float/Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TimelineOverrideExpressionFloatConstant : TimelineOverrideExpressionFloat
    {
        public float Value;

        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Value;
        }
    }

    /// <summary>
    /// floatの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("float/Value")]
    public class TimelineOverrideExpressionFloatValue : TimelineOverrideExpressionFloat
    {
        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return ((TweenTimelineField<float>)field).Value;
        }
    }

    /// <summary>
    /// floatの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("float/Predefined Value")]
    public class TimelineOverrideExpressionFloatPredefinedValue : TimelineOverrideExpressionFloat
    {
        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return ((TweenTimelineField<float>)field).PredefinedValue;
        }
    }

    /// <summary>
    /// floatの値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("float/Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineOverrideExpressionFloatParameter : TimelineOverrideExpressionFloat, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return parameter.Float.GetOrDefault(paramHash);
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            paramHash = TweenParameter.StringToHash(ParameterName);
        }
    }

    /// <summary>
    /// floatの値表現 (Add)
    /// </summary>
    [Serializable]
    [Name("float/Add")]
    public class TimelineOverrideExpressionFloatAdd : TimelineOverrideExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionFloat Left = new TimelineOverrideExpressionFloatConstant();
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionFloat Right = new TimelineOverrideExpressionFloatConstant();
        
        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) + Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// floatの値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("float/Subtract")]
    public class TimelineOverrideExpressionFloatSubtract : TimelineOverrideExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionFloat Left = new TimelineOverrideExpressionFloatConstant();

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionFloat Right = new TimelineOverrideExpressionFloatConstant();

        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) - Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// floatの値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("float/Multiply")]
    public class TimelineOverrideExpressionFloatMultiply : TimelineOverrideExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionFloat Left = new TimelineOverrideExpressionFloatConstant();
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionFloat Right = new TimelineOverrideExpressionFloatConstant();
        
        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) * Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// floatの値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("float/Divide")]
    public class TimelineOverrideExpressionFloatDivide : TimelineOverrideExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionFloat Left = new TimelineOverrideExpressionFloatConstant();

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionFloat Right = new TimelineOverrideExpressionFloatConstant();

        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) / Right.GetValue(parameter, field);
        }
    }
}
