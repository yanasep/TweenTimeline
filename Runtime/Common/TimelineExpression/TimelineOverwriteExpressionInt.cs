using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// intの値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineOverwriteExpressionInt : TimelineOverwriteExpression<int>
    {
    }

    /// <summary>
    /// intの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("int/Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TimelineOverwriteExpressionIntConstant : TimelineOverwriteExpressionInt
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
    public class TimelineOverwriteExpressionIntParameter : TimelineOverwriteExpressionInt, ISerializationCallbackReceiver
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
    public class TimelineOverwriteExpressionIntAdd : TimelineOverwriteExpressionInt
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionInt Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionInt Right;
        
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
    public class TimelineOverwriteExpressionIntSubtract : TimelineOverwriteExpressionInt
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionInt Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionInt Right;

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
    public class TimelineOverwriteExpressionIntMultiply : TimelineOverwriteExpressionInt
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionInt Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionInt Right;
        
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
    public class TimelineOverwriteExpressionIntDivide : TimelineOverwriteExpressionInt
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionInt Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionInt Right;

        /// <inheritdoc/>
        public override int GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) / Right.GetValue(parameter, field);
        }
    }
}
