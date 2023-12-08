using System;
using System.ComponentModel;
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
    [DisplayName("Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TweenTimelineExpressionIntConstant : TweenTimelineExpressionInt
    {
        public int Value;

        public TweenTimelineExpressionIntConstant() { }
        public TweenTimelineExpressionIntConstant(int value) => Value = value;
    
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
    [DisplayName("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TweenTimelineExpressionIntParameter : TweenTimelineExpressionInt, ISerializationCallbackReceiver
    {
        [TweenParameterNameField(typeof(int))]
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
    [DisplayName("Add")]
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
    [DisplayName("Subtract")]
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
    [DisplayName("Multiply")]
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
    [DisplayName("Divide")]
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
