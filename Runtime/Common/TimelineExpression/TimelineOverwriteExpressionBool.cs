using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// boolの値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineOverwriteExpressionBool : TimelineOverwriteExpression<bool>
    {
    }

    /// <summary>
    /// boolの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("bool/Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TimelineOverwriteExpressionBoolConstant : TimelineOverwriteExpressionBool
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
    public class TimelineOverwriteExpressionBoolParameter : TimelineOverwriteExpressionBool, ISerializationCallbackReceiver
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
    public class TimelineOverwriteExpressionBoolAnd : TimelineOverwriteExpressionBool
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionBool Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionBool Right;
        
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
    public class TimelineOverwriteExpressionBoolOr : TimelineOverwriteExpressionBool
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionBool Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpressionBool Right;

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
    public class TimelineOverwriteExpressionBoolNot : TimelineOverwriteExpressionBool
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineOverwriteExpressionBool Value;
        
        /// <inheritdoc/>
        public override bool GetValue(TweenParameter parameter, TweenTimelineField field)
        {
            return !Value.GetValue(parameter, field);
        }
    }
}
