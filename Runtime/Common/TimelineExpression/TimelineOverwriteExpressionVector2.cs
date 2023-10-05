using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Vector2の値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineOverwriteExpressionVector2 : TimelineOverwriteExpression<Vector2>
    {
    }

    /// <summary>
    /// Vector2の値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Vector2/Constant")]
    public class TimelineOverwriteExpressionVector2Constant : TimelineOverwriteExpressionVector2
    {
        [NoPropertyLabel] public Vector2 Value;

        /// <inheritdoc/>
        public override Vector2 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Value;
        }
    }

    /// <summary>
    /// Vector2の値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("Vector2/Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineOverwriteExpressionVector2Parameter : TimelineOverwriteExpressionVector2, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override Vector2 GetValue(TweenParameter parameter, TweenTimelineField field)
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
    [Name("Vector2/Add")]
    public class TimelineOverwriteExpressionVector2Add : TimelineOverwriteExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionVector2 Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionVector2 Right;
        
        /// <inheritdoc/>
        public override Vector2 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) + Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// Vector2の値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("Vector2/Subtract")]
    public class TimelineOverwriteExpressionVector2Subtract : TimelineOverwriteExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector2 Right;

        /// <inheritdoc/>
        public override Vector2 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) - Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// Vector2の値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("Vector2/Multiply")]
    public class TimelineOverwriteExpressionVector2Multiply : TimelineOverwriteExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionVector2 Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionFloat Right;
        
        /// <inheritdoc/>
        public override Vector2 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) * Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// Vector2の値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("Vector2/Divide")]
    public class TimelineOverwriteExpressionVector2Divide : TimelineOverwriteExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionFloat Right;

        /// <inheritdoc/>
        public override Vector2 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) / Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// Vector2の値表現 (Scale)
    /// </summary>
    [Serializable]
    [Name("Vector2/Scale")]
    public class TimelineOverwriteExpressionVector2Scale : TimelineOverwriteExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector2 Right;

        /// <inheritdoc/>
        public override Vector2 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Vector2.Scale(Left.GetValue(parameter, field), Right.GetValue(parameter, field));
        }
    }
}