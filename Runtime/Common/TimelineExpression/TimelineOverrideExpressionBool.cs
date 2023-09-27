using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// boolの値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineOverrideExpressionBool : TimelineOverrideExpression<bool>
    {
    }

    /// <summary>
    /// boolの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("bool/Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TimelineOverrideExpressionBoolConstant : TimelineOverrideExpressionBool
    {
        public bool Value;

        /// <inheritdoc/>
        public override bool GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Value;
        }
    }

    /// <summary>
    /// boolの値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("bool/Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineOverrideExpressionBoolParameter : TimelineOverrideExpressionBool, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override bool GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return parameter.Bool.GetOrDefault(paramHash);
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
    /// boolの値表現 (And)
    /// </summary>
    [Serializable]
    [Name("bool/And")]
    public class TimelineOverrideExpressionBoolAnd : TimelineOverrideExpressionBool
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionBool Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionBool Right;
        
        /// <inheritdoc/>
        public override bool GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) && Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// boolの値表現 (Or)
    /// </summary>
    [Serializable]
    [Name("bool/Or")]
    public class TimelineOverrideExpressionBoolOr : TimelineOverrideExpressionBool
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionBool Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpressionBool Right;

        /// <inheritdoc/>
        public override bool GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return Left.GetValue(parameter, field) || Right.GetValue(parameter, field);
        }
    }

    /// <summary>
    /// Vector3の値表現 (Not)
    /// </summary>
    [Serializable]
    [Name("bool/Not")]
    public class TimelineOverrideExpressionBoolNot : TimelineOverrideExpressionBool
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverrideExpressionBool Value;
        
        /// <inheritdoc/>
        public override bool GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return !Value.GetValue(parameter, field);
        }
    }
}
