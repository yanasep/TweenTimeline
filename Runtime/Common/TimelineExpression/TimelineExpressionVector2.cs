using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Vector2の値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineExpressionVector2
    {
        /// <summary>値取得</summary>
        public abstract Vector2 GetValue(TimelineParameterContainer parameter);
    }

    /// <summary>
    /// Vector2の値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Constant")]
    public class TimelineExpressionVector2Constant : TimelineExpressionVector2
    {
        [NoPropertyLabel] public Vector2 Value;

        /// <inheritdoc/>
        public override Vector2 GetValue(TimelineParameterContainer parameter)
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
    public class TimelineExpressionVector2Parameter : TimelineExpressionVector2, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override Vector2 GetValue(TimelineParameterContainer parameter)
        {
            return parameter.Vector2.GetOrDefault(paramHash);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            paramHash = TimelineParameterContainer.StringToHash(ParameterName);
        }
    }

    /// <summary>
    /// Vector2の値表現 (Add)
    /// </summary>
    [Serializable]
    [Name("Add")]
    public class TimelineExpressionVector2Add : TimelineExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector2 Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector2 Right;
        
        /// <inheritdoc/>
        public override Vector2 GetValue(TimelineParameterContainer parameter)
        {
            return Left.GetValue(parameter) + Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// Vector2の値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("Subtract")]
    public class TimelineExpressionVector2Subtract : TimelineExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector2 Right;

        /// <inheritdoc/>
        public override Vector2 GetValue(TimelineParameterContainer parameter)
        {
            return Left.GetValue(parameter) - Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// Vector2の値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("Multiply")]
    public class TimelineExpressionVector2Multiply : TimelineExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector2 Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionFloat Right;
        
        /// <inheritdoc/>
        public override Vector2 GetValue(TimelineParameterContainer parameter)
        {
            return Left.GetValue(parameter) * Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// Vector2の値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("Divide")]
    public class TimelineExpressionVector2Divide : TimelineExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionFloat Right;

        /// <inheritdoc/>
        public override Vector2 GetValue(TimelineParameterContainer parameter)
        {
            return Left.GetValue(parameter) / Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// Vector2の値表現 (Scale)
    /// </summary>
    [Serializable]
    [Name("Scale")]
    public class TimelineExpressionVector2Scale : TimelineExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector2 Right;

        /// <inheritdoc/>
        public override Vector2 GetValue(TimelineParameterContainer parameter)
        {
            return Vector2.Scale(Left.GetValue(parameter), Right.GetValue(parameter));
        }
    }
}