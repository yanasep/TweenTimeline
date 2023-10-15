using System;
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
    [Name("Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TweenTimelineExpressionBoolConstant : TweenTimelineExpressionBool
    {
        [FormerlySerializedAs("Val")] public bool Value;

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
    [Name("Parameter")]
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
    [Name("And")]
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
    [Name("Or")]
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
    [Name("Not")]
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
