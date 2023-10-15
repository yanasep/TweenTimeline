using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// floatの値表現
    /// </summary>
    [Serializable]
    public abstract class TweenTimelineExpressionFloat : TweenTimelineExpression<float>
    {
    }

    /// <summary>
    /// floatの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("float/Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TweenTimelineExpressionFloatConstant : TweenTimelineExpressionFloat
    {
        public float Value;

        /// <inheritdoc/>
        public override float Evaluate(TweenParameter parameter)
        {
            return Value;
        }
    }

    /// <summary>
    /// floatの値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("float/Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TweenTimelineExpressionFloatParameter : TweenTimelineExpressionFloat, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override float Evaluate(TweenParameter parameter)
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
    public class TweenTimelineExpressionFloatAdd : TweenTimelineExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionFloat Left = new TweenTimelineExpressionFloatConstant();
        
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionFloat Right = new TweenTimelineExpressionFloatConstant();
        
        /// <inheritdoc/>
        public override float Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) + Right.Evaluate(parameter);
        }
    }

    /// <summary>
    /// floatの値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("float/Subtract")]
    public class TweenTimelineExpressionFloatSubtract : TweenTimelineExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Left = new TweenTimelineExpressionFloatConstant();

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Right = new TweenTimelineExpressionFloatConstant();

        /// <inheritdoc/>
        public override float Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) - Right.Evaluate(parameter);
        }
    }

    /// <summary>
    /// floatの値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("float/Multiply")]
    public class TweenTimelineExpressionFloatMultiply : TweenTimelineExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionFloat Left = new TweenTimelineExpressionFloatConstant();
        
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionFloat Right = new TweenTimelineExpressionFloatConstant();
        
        /// <inheritdoc/>
        public override float Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) * Right.Evaluate(parameter);
        }
    }

    /// <summary>
    /// floatの値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("float/Divide")]
    public class TweenTimelineExpressionFloatDivide : TweenTimelineExpressionFloat
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Left = new TweenTimelineExpressionFloatConstant();

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Right = new TweenTimelineExpressionFloatConstant();

        /// <inheritdoc/>
        public override float Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) / Right.Evaluate(parameter);
        }
    }
}
