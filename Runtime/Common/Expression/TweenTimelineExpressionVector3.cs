using System;
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
    [Name("Constant")]
    public class TweenTimelineExpressionVector3Constant : TweenTimelineExpressionVector3
    {
        [NoPropertyLabel] public Vector3 Value;

        /// <inheritdoc/>
        public override Vector3 Evaluate(TweenParameter parameter)
        {
            return Value;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Value == Vector3.zero) return "Vector3.zero";
            return $"new Vector3{Value.ToString()}";
        }
    }

    /// <summary>
    /// Vector3の値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TweenTimelineExpressionVector3Parameter : TweenTimelineExpressionVector3, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override Vector3 Evaluate(TweenParameter parameter)
        {
            return parameter.Vector3.GetOrDefault(paramHash);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            paramHash = TweenParameter.StringToHash(ParameterName);
        }

        public override string ToString()
        {
            return ParameterName;
        }
    }

    /// <summary>
    /// Vector3の値表現 (Add)
    /// </summary>
    [Serializable]
    [Name("Add")]
    public class TweenTimelineExpressionVector3Add : TweenTimelineExpressionVector3
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
    }

    /// <summary>
    /// Vector3の値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("Subtract")]
    public class TweenTimelineExpressionVector3Subtract : TweenTimelineExpressionVector3
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
    }

    /// <summary>
    /// Vector3の値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("Multiply")]
    public class TweenTimelineExpressionVector3Multiply : TweenTimelineExpressionVector3
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
    }

    /// <summary>
    /// Vector3の値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("Divide")]
    public class TweenTimelineExpressionVector3Divide : TweenTimelineExpressionVector3
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
    }

    /// <summary>
    /// Vector3の値表現 (Scale)
    /// </summary>
    [Serializable]
    [Name("Scale")]
    public class TweenTimelineExpressionVector3Scale : TweenTimelineExpressionVector3
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
    }
}
