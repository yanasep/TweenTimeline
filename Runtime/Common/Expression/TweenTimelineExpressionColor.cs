using System;
using System.ComponentModel;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// Colorの値表現
    /// </summary>
    [Serializable]
    public abstract class TweenTimelineExpressionColor : TweenTimelineExpression<Color>
    {
    }

    /// <summary>
    /// Colorの値表現 (Constant)
    /// </summary>
    [Serializable]
    [DisplayName("Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TweenTimelineExpressionColorConstant : TweenTimelineExpressionColor
    {
        public Color Value = Color.white;

        public TweenTimelineExpressionColorConstant() { }
        public TweenTimelineExpressionColorConstant(Color value) => Value = value;

        /// <inheritdoc/>
        public override Color Evaluate(TweenParameter parameter)
        {
            return Value;
        }
    }

    /// <summary>
    /// Colorの値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [DisplayName("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterId))]
    public class TweenTimelineExpressionColorParameter : TweenTimelineExpressionColor
    {
        [TweenParameterIdField(typeof(Color))]
        public uint ParameterId;

        /// <inheritdoc/>
        public override Color Evaluate(TweenParameter parameter)
        {
            return parameter.GetColor(ParameterId);
        }
    }
}
