using System;
using System.ComponentModel;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Vector3の値表現
    /// </summary>
    [Serializable]
    public abstract class TweenTimelineExpressionVector3 : TweenTimelineExpression<Vector3>
    {
    }

    /// <summary>
    /// Vector3の値表現 (Constant)
    /// </summary>
    [Serializable]
    [DisplayName("Constant")]
    public class TweenTimelineExpressionVector3Constant : TweenTimelineExpressionVector3
    {
        [NoPropertyLabel] public Vector3 Value;

        public TweenTimelineExpressionVector3Constant() { }
        public TweenTimelineExpressionVector3Constant(Vector3 val) => Value = val;

        /// <inheritdoc/>
        public override Vector3 Evaluate(TweenParameter parameter)
        {
            return Value;
        }
    }

    /// <summary>
    /// Vector3の値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [DisplayName("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterId))]
    public class TweenTimelineExpressionVector3Parameter : TweenTimelineExpressionVector3
    {
        [TweenParameterIdField(typeof(Vector3))]
        public uint ParameterId;

        /// <inheritdoc/>
        public override Vector3 Evaluate(TweenParameter parameter)
        {
            return parameter.GetVector3(ParameterId);
        }

        public void OnBeforeSerialize()
        {
        }
    }

    /// <summary>
    /// Vector3の値表現 (Add)
    /// </summary>
    [Serializable]
    [DisplayName("Add")]
    public class TweenTimelineExpressionVector3Add : TweenTimelineExpressionVector3, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 Right;

        /// <inheritdoc/>
        public override Vector3 Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) + Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionVector3Constant();
            Right ??= new TweenTimelineExpressionVector3Constant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// Vector3の値表現 (Subtract)
    /// </summary>
    [Serializable]
    [DisplayName("Subtract")]
    public class TweenTimelineExpressionVector3Subtract : TweenTimelineExpressionVector3, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 Right;

        /// <inheritdoc/>
        public override Vector3 Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) - Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionVector3Constant();
            Right ??= new TweenTimelineExpressionVector3Constant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// Vector3の値表現 (Multiply)
    /// </summary>
    [Serializable]
    [DisplayName("Multiply")]
    public class TweenTimelineExpressionVector3Multiply : TweenTimelineExpressionVector3, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Right;

        /// <inheritdoc/>
        public override Vector3 Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) * Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionVector3Constant();
            Right ??= new TweenTimelineExpressionFloatConstant(1);
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// Vector3の値表現 (Divide)
    /// </summary>
    [Serializable]
    [DisplayName("Divide")]
    public class TweenTimelineExpressionVector3Divide : TweenTimelineExpressionVector3, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Right;

        /// <inheritdoc/>
        public override Vector3 Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) / Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionVector3Constant();
            Right ??= new TweenTimelineExpressionFloatConstant(1);
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// Vector3の値表現 (Scale)
    /// </summary>
    [Serializable]
    [DisplayName("Scale")]
    public class TweenTimelineExpressionVector3Scale : TweenTimelineExpressionVector3, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 Right;

        /// <inheritdoc/>
        public override Vector3 Evaluate(TweenParameter parameter)
        {
            return Vector3.Scale(Left.Evaluate(parameter), Right.Evaluate(parameter));
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionVector3Constant();
            Right ??= new TweenTimelineExpressionVector3Constant(Vector3.one);
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }
}
