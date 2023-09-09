using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Colorの値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineExpressionColor
    {
        /// <summary>値取得</summary>
        public abstract Color GetValue(TimelineParameterContainer parameter);
    }

    /// <summary>
    /// Colorの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TimelineExpressionColorConstant : TimelineExpressionColor
    {
        public Color Value;

        /// <inheritdoc/>
        public override Color GetValue(TimelineParameterContainer parameter)
        {
            return Value;
        }
    }

    /// <summary>
    /// Colorの値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineExpressionColorParameter : TimelineExpressionColor, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override Color GetValue(TimelineParameterContainer parameter)
        {
            return parameter.Color.GetOrDefault(paramHash);
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            paramHash = TimelineParameterContainer.StringToHash(ParameterName);
        }
    }
}
