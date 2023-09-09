using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Vector3の値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineExpressionVector3
    {
        /// <summary>値取得</summary>
        public abstract Vector3 GetValue(TimelineParameterContainer parameter);
    }

    /// <summary>
    /// Vector3の値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Constant")]
    public class TimelineExpressionVector3Constant : TimelineExpressionVector3
    {
        [NoPropertyLabel] public Vector3 Value;

        /// <inheritdoc/>
        public override Vector3 GetValue(TimelineParameterContainer parameter)
        {
            return Value;
        }
    }

    /// <summary>
    /// Vector3の値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineExpressionVector3Parameter : TimelineExpressionVector3, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override Vector3 GetValue(TimelineParameterContainer parameter)
        {
            return parameter.Vector3.GetOrDefault(paramHash);
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
    /// Vector3の値表現 (Add)
    /// </summary>
    [Serializable]
    [Name("Add")]
    public class TimelineExpressionVector3Add : TimelineExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 Right;
        
        /// <inheritdoc/>
        public override Vector3 GetValue(TimelineParameterContainer parameter)
        {
            return Left.GetValue(parameter) + Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// Vector3の値表現 (Subtract)
    /// </summary>
    [Serializable]
    [Name("Subtract")]
    public class TimelineExpressionVector3Subtract : TimelineExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector3 Right;

        /// <inheritdoc/>
        public override Vector3 GetValue(TimelineParameterContainer parameter)
        {
            return Left.GetValue(parameter) - Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// Vector3の値表現 (Multiply)
    /// </summary>
    [Serializable]
    [Name("Multiply")]
    public class TimelineExpressionVector3Multiply : TimelineExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionFloat Right;
        
        /// <inheritdoc/>
        public override Vector3 GetValue(TimelineParameterContainer parameter)
        {
            return Left.GetValue(parameter) * Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// Vector3の値表現 (Divide)
    /// </summary>
    [Serializable]
    [Name("Divide")]
    public class TimelineExpressionVector3Divide : TimelineExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionFloat Right;

        /// <inheritdoc/>
        public override Vector3 GetValue(TimelineParameterContainer parameter)
        {
            return Left.GetValue(parameter) / Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// Vector3の値表現 (Scale)
    /// </summary>
    [Serializable]
    [Name("Scale")]
    public class TimelineExpressionVector3Scale : TimelineExpressionVector3
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector3 Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector3 Right;

        /// <inheritdoc/>
        public override Vector3 GetValue(TimelineParameterContainer parameter)
        {
            return Vector3.Scale(Left.GetValue(parameter), Right.GetValue(parameter));
        }
    }
}