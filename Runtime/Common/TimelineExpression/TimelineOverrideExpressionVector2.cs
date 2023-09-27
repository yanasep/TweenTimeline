using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Vector2の値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineOverrideExpressionVector2 : TimelineOverrideExpression<Vector2>
    {
    }

    /// <summary>
    /// Vector2の値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Vector2/Constant")]
    public class TimelineOverrideExpressionVector2Constant : TimelineOverrideExpressionVector2
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
    [Name("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineOverrideExpressionVector2Parameter : TimelineOverrideExpressionVector2, ISerializationCallbackReceiver
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
    [Name("Add")]
    public class TimelineOverrideExpressionVector2Add : TimelineOverrideExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionVector2 Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionVector2 Right;
        
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
    [Name("Subtract")]
    public class TimelineOverrideExpressionVector2Subtract : TimelineOverrideExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector2 Right;

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
    [Name("Multiply")]
    public class TimelineOverrideExpressionVector2Multiply : TimelineOverrideExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionVector2 Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionFloat Right;
        
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
    [Name("Divide")]
    public class TimelineOverrideExpressionVector2Divide : TimelineOverrideExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionFloat Right;

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
    [Name("Scale")]
    public class TimelineOverrideExpressionVector2Scale : TimelineOverrideExpressionVector2
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector2 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector2 Right;

        /// <inheritdoc/>
        public override Vector2 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Vector2.Scale(Left.GetValue(parameter, field), Right.GetValue(parameter, field));
        }
    }
}