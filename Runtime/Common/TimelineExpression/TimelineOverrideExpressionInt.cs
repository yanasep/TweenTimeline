using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// intの値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineOverrideExpressionInt : TimelineOverrideExpression<int>
    {
    }

    /// <summary>
    /// intの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("int/Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TimelineOverrideExpressionIntConstant : TimelineOverrideExpressionInt
    {
        public int Value;

        /// <inheritdoc/>
        public override int GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Value;
        }
    }

    /// <summary>
    /// intの値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("int/Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineOverrideExpressionIntParameter : TimelineOverrideExpressionInt, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override int GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return parameter.Int.GetOrDefault(paramHash);
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
    /// intの値表現 (Add)
    /// </summary>
    [Serializable]
    [Name("int/Add")]
    public class TimelineOverrideExpressionIntAdd : TimelineOverrideExpressionInt
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionInt Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionInt Right;
        
        /// <inheritdoc/>
        public override int GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) + Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// intの値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("int/Subtract")]
    public class TimelineOverrideExpressionIntSubtract : TimelineOverrideExpressionInt
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionInt Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionInt Right;

        /// <inheritdoc/>
        public override int GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) - Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// intの値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("int/Multiply")]
    public class TimelineOverrideExpressionIntMultiply : TimelineOverrideExpressionInt
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionInt Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionInt Right;
        
        /// <inheritdoc/>
        public override int GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) * Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// intの値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("int/Divide")]
    public class TimelineOverrideExpressionIntDivide : TimelineOverrideExpressionInt
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionInt Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionInt Right;

        /// <inheritdoc/>
        public override int GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) / Right.GetValue(parameter, field);
        }
    }
}
