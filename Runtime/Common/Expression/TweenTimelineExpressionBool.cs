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
    [SelectableSerializeSingleLine(nameof(ParameterId))]
    public class TweenTimelineExpressionBoolParameter : TweenTimelineExpressionBool
    {
        [TweenParameterIdField(typeof(bool))] 
        public uint ParameterId;

        /// <inheritdoc/>
        public override bool Evaluate(TweenParameter parameter)
        {
            return parameter.GetBool(ParameterId);
        }
    }

    /// <summary>
    /// boolの値表現 (And)
    /// </summary>
    [Serializable]
    [DisplayName("And")]
    public class TweenTimelineExpressionBoolAnd : TweenTimelineExpressionBool, ISerializationCallbackReceiver
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

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionBoolConstant();
            Right ??= new TweenTimelineExpressionBoolConstant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// boolの値表現 (Or)
    /// </summary>
    [Serializable]
    [DisplayName("Or")]
    public class TweenTimelineExpressionBoolOr : TweenTimelineExpressionBool, ISerializationCallbackReceiver
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

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionBoolConstant();
            Right ??= new TweenTimelineExpressionBoolConstant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// Vector3の値表現 (Not)
    /// </summary>
    [Serializable]
    [DisplayName("Not")]
    public class TweenTimelineExpressionBoolNot : TweenTimelineExpressionBool, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionBool Value;
        
        /// <inheritdoc/>
        public override bool Evaluate(TweenParameter parameter)
        {
            return !Value.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Value ??= new TweenTimelineExpressionBoolConstant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }
}
