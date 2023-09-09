using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// floatの値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineExpressionFloat
    {
        /// <summary>値取得</summary>
        public abstract float GetValue(TweenParameter parameter);
    }

    /// <summary>
    /// floatの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TimelineExpressionFloatConstant : TimelineExpressionFloat
    {
        public float Value;

        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter)
        {
            return Value;
        }
    }

    /// <summary>
    /// floatの値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineExpressionFloatParameter : TimelineExpressionFloat, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter)
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
    [Name("Add")]
    public class TimelineExpressionFloatAdd : TimelineExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionFloat Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionFloat Right;
        
        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter)
        {
            return Left.GetValue(parameter) + Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// floatの値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("Subtract")]
    public class TimelineExpressionFloatSubtract : TimelineExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionFloat Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionFloat Right;

        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter)
        {
            return Left.GetValue(parameter) - Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// floatの値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("Multiply")]
    public class TimelineExpressionFloatMultiply : TimelineExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionFloat Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionFloat Right;
        
        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter)
        {
            return Left.GetValue(parameter) * Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// floatの値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("Divide")]
    public class TimelineExpressionFloatDivide : TimelineExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionFloat Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionFloat Right;

        /// <inheritdoc/>
        public override float GetValue(TweenParameter parameter)
        {
            return Left.GetValue(parameter) / Right.GetValue(parameter);
        }
    }
}
