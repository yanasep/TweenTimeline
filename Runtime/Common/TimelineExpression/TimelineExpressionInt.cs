using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// intの値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineExpressionInt
    {
        /// <summary>値取得</summary>
        public abstract int GetValue(TweenParameterContainer parameter);
    }

    /// <summary>
    /// intの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TimelineExpressionIntConstant : TimelineExpressionInt
    {
        public int Value;

        /// <inheritdoc/>
        public override int GetValue(TweenParameterContainer parameter)
        {
            return Value;
        }
    }

    /// <summary>
    /// intの値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineExpressionIntParameter : TimelineExpressionInt, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override int GetValue(TweenParameterContainer parameter)
        {
            return parameter.Int.GetOrDefault(paramHash);
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            paramHash = TweenParameterContainer.StringToHash(ParameterName);
        }
    }

    /// <summary>
    /// intの値表現 (Add)
    /// </summary>
    [Serializable]
    [Name("Add")]
    public class TimelineExpressionIntAdd : TimelineExpressionInt
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionInt Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionInt Right;
        
        /// <inheritdoc/>
        public override int GetValue(TweenParameterContainer parameter)
        {
            return Left.GetValue(parameter) + Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// intの値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("Subtract")]
    public class TimelineExpressionIntSubtract : TimelineExpressionInt
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionInt Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionInt Right;

        /// <inheritdoc/>
        public override int GetValue(TweenParameterContainer parameter)
        {
            return Left.GetValue(parameter) - Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// intの値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("Multiply")]
    public class TimelineExpressionIntMultiply : TimelineExpressionInt
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionInt Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionInt Right;
        
        /// <inheritdoc/>
        public override int GetValue(TweenParameterContainer parameter)
        {
            return Left.GetValue(parameter) * Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// intの値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("Divide")]
    public class TimelineExpressionIntDivide : TimelineExpressionInt
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionInt Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionInt Right;

        /// <inheritdoc/>
        public override int GetValue(TweenParameterContainer parameter)
        {
            return Left.GetValue(parameter) / Right.GetValue(parameter);
        }
    }
}
