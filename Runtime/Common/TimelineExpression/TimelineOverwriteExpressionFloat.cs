using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// floatの値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineOverwriteExpressionFloat : TimelineOverwriteExpression<float>
    {
    }

    /// <summary>
    /// floatの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("float/Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TimelineOverwriteExpressionFloatConstant : TimelineOverwriteExpressionFloat
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
    public class TimelineOverwriteExpressionFloatValue : TimelineOverwriteExpressionFloat
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
    public class TimelineOverwriteExpressionFloatPredefinedValue : TimelineOverwriteExpressionFloat
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
    public class TimelineOverwriteExpressionFloatParameter : TimelineOverwriteExpressionFloat, ISerializationCallbackReceiver
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
    public class TimelineOverwriteExpressionFloatAdd : TimelineOverwriteExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionFloat Left = new TimelineOverwriteExpressionFloatConstant();
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionFloat Right = new TimelineOverwriteExpressionFloatConstant();
        
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
    public class TimelineOverwriteExpressionFloatSubtract : TimelineOverwriteExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionFloat Left = new TimelineOverwriteExpressionFloatConstant();

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionFloat Right = new TimelineOverwriteExpressionFloatConstant();

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
    public class TimelineOverwriteExpressionFloatMultiply : TimelineOverwriteExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionFloat Left = new TimelineOverwriteExpressionFloatConstant();
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionFloat Right = new TimelineOverwriteExpressionFloatConstant();
        
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
    public class TimelineOverwriteExpressionFloatDivide : TimelineOverwriteExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionFloat Left = new TimelineOverwriteExpressionFloatConstant();

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionFloat Right = new TimelineOverwriteExpressionFloatConstant();

        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) / Right.GetValue(parameter, field);
        }
    }
}
