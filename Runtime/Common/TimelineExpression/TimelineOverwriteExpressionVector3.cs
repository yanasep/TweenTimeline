using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Vector3の値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineOverwriteExpressionVector3 : TimelineOverwriteExpression<Vector3>
    {
    }

    /// <summary>
    /// Vector3の値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Vector3/Constant")]
    public class TimelineOverwriteExpressionVector3Constant : TimelineOverwriteExpressionVector3
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
    public class TimelineOverwriteExpressionVector3Parameter : TimelineOverwriteExpressionVector3, ISerializationCallbackReceiver
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
    public class TimelineOverwriteExpressionVector3Add : TimelineOverwriteExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector3 Right;

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
    public class TimelineOverwriteExpressionVector3Subtract : TimelineOverwriteExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector3 Right;

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
    public class TimelineOverwriteExpressionVector3Multiply : TimelineOverwriteExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionFloat Right;

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
    public class TimelineOverwriteExpressionVector3Divide : TimelineOverwriteExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionFloat Right;

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
    public class TimelineOverwriteExpressionVector3Scale : TimelineOverwriteExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionVector3 Right;

        /// <inheritdoc/>
        public override Vector3 GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Vector3.Scale(Left.GetValue(parameter, field), Right.GetValue(parameter, field));
        }
    }
}
