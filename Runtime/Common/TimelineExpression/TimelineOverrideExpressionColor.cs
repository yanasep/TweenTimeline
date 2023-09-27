using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Colorの値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineOverrideExpressionColor : TimelineOverrideExpression<Color>
    {
    }

    /// <summary>
    /// Colorの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Color/Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TimelineOverrideExpressionColorConstant : TimelineOverrideExpressionColor
    {
        public Color Value;

        /// <inheritdoc/>
        public override Color GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Value;
        }
    }

    /// <summary>
    /// Colorの値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("Color/Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineOverrideExpressionColorParameter : TimelineOverrideExpressionColor, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override Color GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return parameter.Color.GetOrDefault(paramHash);
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            paramHash = TweenParameter.StringToHash(ParameterName);
        }
    }
}
