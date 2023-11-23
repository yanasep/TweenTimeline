using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// boolの値表現
    /// </summary>
    [Serializable]
    public abstract class TweenTimelineExpressionBool : TweenTimelineExpression<bool>
    {
    }

    /// <summary>
    /// boolの値表現 (Constant)
    /// </summary>
    [Serializable]
    [DisplayName("Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TweenTimelineExpressionBoolConstant : TweenTimelineExpressionBool
    {
        [FormerlySerializedAs("Val")] public bool Value;
        
        public TweenTimelineExpressionBoolConstant() { }
        public TweenTimelineExpressionBoolConstant(bool value) => Value = value;

        /// <inheritdoc/>
        public override bool Evaluate(TweenParameter parameter)
        {
            return Value;
        }
    }

    /// <summary>
    /// boolの値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [DisplayName("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TweenTimelineExpressionBoolParameter : TweenTimelineExpressionBool, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override bool Evaluate(TweenParameter parameter)
        {
            return parameter.Bool.GetOrDefault(paramHash);
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
    /// boolの値表現 (And)
    /// </summary>
    [Serializable]
    [DisplayName("And")]
    public class TweenTimelineExpressionBoolAnd : TweenTimelineExpressionBool
    {
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionBool Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionBool Right;
        
        /// <inheritdoc/>
        public override bool Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) && Right.Evaluate(parameter);
        }
    }

    /// <summary>
    /// boolの値表現 (Or)
    /// </summary>
    [Serializable]
    [DisplayName("Or")]
    public class TweenTimelineExpressionBoolOr : TweenTimelineExpressionBool
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionBool Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionBool Right;

        /// <inheritdoc/>
        public override bool Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) || Right.Evaluate(parameter);
        }
    }

    /// <summary>
    /// Vector3の値表現 (Not)
    /// </summary>
    [Serializable]
    [DisplayName("Not")]
    public class TweenTimelineExpressionBoolNot : TweenTimelineExpressionBool
    {
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionBool Value;
        
        /// <inheritdoc/>
        public override bool Evaluate(TweenParameter parameter)
        {
            return !Value.Evaluate(parameter);
        }
    }
}
