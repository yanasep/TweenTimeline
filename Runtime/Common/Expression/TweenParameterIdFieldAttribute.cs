using System;
using UnityEngine;

namespace TweenTimeline
{
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
