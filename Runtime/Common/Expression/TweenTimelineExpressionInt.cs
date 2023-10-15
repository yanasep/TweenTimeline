using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// intの値表現
    /// </summary>
    [Serializable]
    public abstract class TweenTimelineExpressionInt : TweenTimelineExpression<int>
    {
    }

    /// <summary>
    /// intの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TweenTimelineExpressionIntConstant : TweenTimelineExpressionInt
    {
        public int Value;
    
        /// <inheritdoc/>
        public override int Evaluate(TweenParameter parameter)
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
    public class TweenTimelineExpressionIntParameter : TweenTimelineExpressionInt, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override int Evaluate(TweenParameter parameter)
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
    [Name("Add")]
    public class TweenTimelineExpressionIntAdd : TweenTimelineExpressionInt
    {
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionInt Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionInt Right;
        
        /// <inheritdoc/>
        public override int Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) + Right.Evaluate(parameter);
        }
    }

    /// <summary>
    /// intの値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("Subtract")]
    public class TweenTimelineExpressionIntSubtract : TweenTimelineExpressionInt
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionInt Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionInt Right;

        /// <inheritdoc/>
        public override int Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) - Right.Evaluate(parameter);
        }
    }

    /// <summary>
    /// intの値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("Multiply")]
    public class TweenTimelineExpressionIntMultiply : TweenTimelineExpressionInt
    {
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionInt Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionInt Right;
        
        /// <inheritdoc/>
        public override int Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) * Right.Evaluate(parameter);
        }
    }

    /// <summary>
    /// intの値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("Divide")]
    public class TweenTimelineExpressionIntDivide : TweenTimelineExpressionInt
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionInt Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionInt Right;

        /// <inheritdoc/>
        public override int Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) / Right.Evaluate(parameter);
        }
    }
}
