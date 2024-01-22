using System;
using System.ComponentModel;
using UnityEngine;

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
    [SelectableSerializeSingleLine(nameof(ParameterId))]
    public class TweenTimelineExpressionIntParameter : TweenTimelineExpressionInt
    {
        [TweenParameterIdField(typeof(int))]
        public uint ParameterId;

        /// <inheritdoc/>
        public override int Evaluate(TweenParameter parameter)
        {
            return parameter.GetInt(ParameterId);
        }
    }

    /// <summary>
    /// intの値表現 (Add)
    /// </summary>
    [Serializable]
    [DisplayName("Add")]
    public class TweenTimelineExpressionIntAdd : TweenTimelineExpressionInt, ISerializationCallbackReceiver
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

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionIntConstant();
            Right ??= new TweenTimelineExpressionIntConstant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// intの値表現 (Subtract)
    /// </summary>
    [Serializable]
    [DisplayName("Subtract")]
    public class TweenTimelineExpressionIntSubtract : TweenTimelineExpressionInt, ISerializationCallbackReceiver
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

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionIntConstant();
            Right ??= new TweenTimelineExpressionIntConstant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// intの値表現 (Multiply)
    /// </summary>
    [Serializable]
    [DisplayName("Multiply")]
    public class TweenTimelineExpressionIntMultiply : TweenTimelineExpressionInt, ISerializationCallbackReceiver
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

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionIntConstant();
            Right ??= new TweenTimelineExpressionIntConstant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// intの値表現 (Divide)
    /// </summary>
    [Serializable]
    [DisplayName("Divide")]
    public class TweenTimelineExpressionIntDivide : TweenTimelineExpressionInt, ISerializationCallbackReceiver
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

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionIntConstant();
            Right ??= new TweenTimelineExpressionIntConstant(1);
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }
}
