using System;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// TweenParameterId(uint)をDropdownで設定するためのAttribute<br/>
    /// TimelineAsset内でシリアライズされるものでのみ利用可能
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TweenParameterIdFieldAttribute : PropertyAttribute
    {
        public readonly Type ParameterType;

        public TweenParameterIdFieldAttribute(Type parameterType)
        {
            ParameterType = parameterType;
        }
    }
}
