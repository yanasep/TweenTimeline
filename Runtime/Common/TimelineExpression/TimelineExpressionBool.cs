using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// boolの値表現
    /// </summary>
    [Serializable]
    public abstract class TimelineExpressionBool
    {
        /// <summary>値取得</summary>
        public abstract bool GetValue(TweenParameterContainer parameter);
    }

    /// <summary>
    /// boolの値表現 (Constant)
    /// </summary>
    [Serializable]
    [Name("Constant")]
    [SelectableSerializeSingleLine(nameof(Value))]
    public class TimelineExpressionBoolConstant : TimelineExpressionBool
    {
        public bool Value;

        /// <inheritdoc/>
        public override bool GetValue(TweenParameterContainer parameter)
        {
            return Value;
        }
    }

    /// <summary>
    /// boolの値表現 (Parameter取得)
    /// </summary>
    [Serializable]
    [Name("Parameter")]
    [SelectableSerializeSingleLine(nameof(ParameterName))]
    public class TimelineExpressionBoolParameter : TimelineExpressionBool, ISerializationCallbackReceiver
    {
        public string ParameterName;
        private int paramHash;

        /// <inheritdoc/>
        public override bool GetValue(TweenParameterContainer parameter)
        {
            return parameter.Bool.GetOrDefault(paramHash);
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            paramHash = TweenParameterContainer.StringToHash(ParameterName);
        }
    }

    /// <summary>
    /// boolの値表現 (And)
    /// </summary>
    [Serializable]
    [Name("And")]
    public class TimelineExpressionBoolAnd : TimelineExpressionBool
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionBool Left;
        
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionBool Right;
        
        /// <inheritdoc/>
        public override bool GetValue(TweenParameterContainer parameter)
        {
            return Left.GetValue(parameter) && Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// boolの値表現 (Or)
    /// </summary>
    [Serializable]
    [Name("Or")]
    public class TimelineExpressionBoolOr : TimelineExpressionBool
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionBool Left;

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionBool Right;

        /// <inheritdoc/>
        public override bool GetValue(TweenParameterContainer parameter)
        {
            return Left.GetValue(parameter) || Right.GetValue(parameter);
        }
    }

    /// <summary>
    /// Vector3の値表現 (Not)
    /// </summary>
    [Serializable]
    [Name("Not")]
    public class TimelineExpressionBoolNot : TimelineExpressionBool
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionBool Value;
        
        /// <inheritdoc/>
        public override bool GetValue(TweenParameterContainer parameter)
        {
            return !Value.GetValue(parameter);
        }
    }
}
