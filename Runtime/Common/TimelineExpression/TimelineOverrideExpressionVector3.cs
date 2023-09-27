using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Vector3の値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineOverrideExpressionVector3 : TimelineOverrideExpression<Vector3>
    {
    }

    /// <summary>
    /// Vector3の値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Vector3/Constant")]
    public class TimelineOverrideExpressionVector3Constant : TimelineOverrideExpressionVector3
    {
        [NoPropertyLabel] public Vector3 Value;

        /// <inheritdoc/>
        public override Vector3 GetValue(TweenParameter parameter, TweenTimelineField field)
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
    [Name("Vector3/Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineOverrideExpressionVector3Parameter : TimelineOverrideExpressionVector3, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override Vector3 GetValue(TweenParameter parameter, TweenTimelineField field)
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
    [Name("Vector3/Add")]
    public class TimelineOverrideExpressionVector3Add : TimelineOverrideExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector3 Right;

        /// <inheritdoc/>
        public override Vector3 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) + Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// Vector3の値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("Vector3/Subtract")]
    public class TimelineOverrideExpressionVector3Subtract : TimelineOverrideExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector3 Right;

        /// <inheritdoc/>
        public override Vector3 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) - Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// Vector3の値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("Vector3/Multiply")]
    public class TimelineOverrideExpressionVector3Multiply : TimelineOverrideExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionFloat Right;

        /// <inheritdoc/>
        public override Vector3 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) * Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// Vector3の値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("Vector3/Divide")]
    public class TimelineOverrideExpressionVector3Divide : TimelineOverrideExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionFloat Right;

        /// <inheritdoc/>
        public override Vector3 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) / Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// Vector3の値表現 (Scale)
    /// </summary>
    [Serializable]
    [Name("Vector3/Scale")]
    public class TimelineOverrideExpressionVector3Scale : TimelineOverrideExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionVector3 Right;

        /// <inheritdoc/>
        public override Vector3 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Vector3.Scale(Left.GetValue(parameter, field), Right.GetValue(parameter, field));
        }
    }
}
