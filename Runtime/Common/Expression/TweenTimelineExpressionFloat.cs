using System;
using System.ComponentModel;
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
    [DisplayName("Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TweenTimelineExpressionFloatConstant : TweenTimelineExpressionFloat
    {
        public float Value;

        public TweenTimelineExpressionFloatConstant() { }
        public TweenTimelineExpressionFloatConstant(float value) => Value = value;

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
    [DisplayName("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterId))]
    public class TweenTimelineExpressionFloatParameter : TweenTimelineExpressionFloat
    {
        [TweenParameterIdField(typeof(float))]
        public string ParameterId;

        /// <inheritdoc/>
        public override float Evaluate(TweenParameter parameter)
        {
            return parameter.GetFloat(ParameterId);
        }
    }

    /// <summary>
    /// floatの値表現 (Add)
    /// </summary>
    [Serializable]
    [DisplayName("Add")]
    public class TweenTimelineExpressionFloatAdd : TweenTimelineExpressionFloat, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Right;
        
        /// <inheritdoc/>
        public override float Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) + Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionFloatConstant();
            Right ??= new TweenTimelineExpressionFloatConstant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// floatの値表現 (Subtract)
    /// </summary>
    [Serializable]
    [DisplayName("Subtract")]
    public class TweenTimelineExpressionFloatSubtract : TweenTimelineExpressionFloat, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Right;

        /// <inheritdoc/>
        public override float Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) - Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionFloatConstant();
            Right ??= new TweenTimelineExpressionFloatConstant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// floatの値表現 (Multiply)
    /// </summary>
    [Serializable]
    [DisplayName("Multiply")]
    public class TweenTimelineExpressionFloatMultiply : TweenTimelineExpressionFloat, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Right;
        
        /// <inheritdoc/>
        public override float Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) * Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionFloatConstant();
            Right ??= new TweenTimelineExpressionFloatConstant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// floatの値表現 (Divide)
    /// </summary>
    [Serializable]
    [DisplayName("Divide")]
    public class TweenTimelineExpressionFloatDivide : TweenTimelineExpressionFloat, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Right;

        /// <inheritdoc/>
        public override float Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) / Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionFloatConstant();
            Right ??= new TweenTimelineExpressionFloatConstant(1);
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }
}
