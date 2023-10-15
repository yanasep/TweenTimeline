using System;
using UnityEngine;
using Yanasep;

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
    [Name("Constant")]
    public class TweenTimelineExpressionVector2Constant : TweenTimelineExpressionVector2
    {
        [NoPropertyLabel] public Vector2 Value;

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
    [Name("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TweenTimelineExpressionVector2Parameter : TweenTimelineExpressionVector2, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override Vector2 Evaluate(TweenParameter parameter)
        {
            return parameter.Vector2.GetOrDefault(paramHash);
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
    /// Vector2の値表現 (Add)
    /// </summary>
    [Serializable]
    [Name("Add")]
    public class TweenTimelineExpressionVector2Add : TweenTimelineExpressionVector2
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
    }

    /// <summary>
    /// Vector2の値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("Subtract")]
    public class TweenTimelineExpressionVector2Subtract : TweenTimelineExpressionVector2
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
    }

    /// <summary>
    /// Vector2の値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("Multiply")]
    public class TweenTimelineExpressionVector2Multiply : TweenTimelineExpressionVector2
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
    }

    /// <summary>
    /// Vector2の値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("Divide")]
    public class TweenTimelineExpressionVector2Divide : TweenTimelineExpressionVector2
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
    }

    /// <summary>
    /// Vector2の値表現 (Scale)
    /// </summary>
    [Serializable]
    [Name("Scale")]
    public class TweenTimelineExpressionVector2Scale : TweenTimelineExpressionVector2
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
    }
}