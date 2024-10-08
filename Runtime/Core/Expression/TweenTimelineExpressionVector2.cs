using System;
using System.ComponentModel;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// Vector2の値表現
    /// </summary>
    [Serializable]
    public abstract class TweenTimelineExpressionVector2 : TweenTimelineExpression<Vector2>
    {
    }

    /// <summary>
    /// Vector2の値表現 (Constant)
    /// </summary>
    [Serializable]
    [DisplayName("Constant")]
    public class TweenTimelineExpressionVector2Constant : TweenTimelineExpressionVector2
    {
        [NoPropertyLabel] public Vector2 Value;

        public TweenTimelineExpressionVector2Constant() { }
        public TweenTimelineExpressionVector2Constant(Vector2 value) => Value = value;

        /// <inheritdoc/>
        public override Vector2 Evaluate(TweenParameter parameter)
        {
            return Value;
        }
    }

    /// <summary>
    /// Vector2の値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [DisplayName("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterId))]
    public class TweenTimelineExpressionVector2Parameter : TweenTimelineExpressionVector2
    {
        [TweenParameterIdField(typeof(Vector2))]
        public uint ParameterId;

        /// <inheritdoc/>
        public override Vector2 Evaluate(TweenParameter parameter)
        {
            return parameter.GetVector2(ParameterId);
        }
    }

    /// <summary>
    /// Vector2の値表現 (Add)
    /// </summary>
    [Serializable]
    [DisplayName("Add")]
    public class TweenTimelineExpressionVector2Add : TweenTimelineExpressionVector2, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionVector2 Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionVector2 Right;
        
        /// <inheritdoc/>
        public override Vector2 Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) + Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionVector2Constant();
            Right ??= new TweenTimelineExpressionVector2Constant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// Vector2の値表現 (Subtract)
    /// </summary>
    [Serializable]
    [DisplayName("Subtract")]
    public class TweenTimelineExpressionVector2Subtract : TweenTimelineExpressionVector2, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector2 Right;

        /// <inheritdoc/>
        public override Vector2 Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) - Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionVector2Constant();
            Right ??= new TweenTimelineExpressionVector2Constant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// Vector2の値表現 (Multiply)
    /// </summary>
    [Serializable]
    [DisplayName("Multiply")]
    public class TweenTimelineExpressionVector2Multiply : TweenTimelineExpressionVector2, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionVector2 Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionFloat Right;
        
        /// <inheritdoc/>
        public override Vector2 Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) * Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionVector2Constant();
            Right ??= new TweenTimelineExpressionFloatConstant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// Vector2の値表現 (Divide)
    /// </summary>
    [Serializable]
    [DisplayName("Divide")]
    public class TweenTimelineExpressionVector2Divide : TweenTimelineExpressionVector2, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat Right;

        /// <inheritdoc/>
        public override Vector2 Evaluate(TweenParameter parameter)
        {
            return Left.Evaluate(parameter) / Right.Evaluate(parameter);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionVector2Constant();
            Right ??= new TweenTimelineExpressionFloatConstant(1);
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }

    /// <summary>
    /// Vector2の値表現 (Scale)
    /// </summary>
    [Serializable]
    [DisplayName("Scale")]
    public class TweenTimelineExpressionVector2Scale : TweenTimelineExpressionVector2, ISerializationCallbackReceiver
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector2 Right;

        /// <inheritdoc/>
        public override Vector2 Evaluate(TweenParameter parameter)
        {
            return Vector2.Scale(Left.Evaluate(parameter), Right.Evaluate(parameter));
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            Left ??= new TweenTimelineExpressionVector2Constant();
            Right ??= new TweenTimelineExpressionVector2Constant();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
        }
    }
}