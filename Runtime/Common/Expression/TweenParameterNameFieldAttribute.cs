using System;
using UnityEngine;

namespace TweenTimeline
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TweenParameterNameFieldAttribute : PropertyAttribute
    {
        public readonly Type ParameterType;

        public TweenParameterNameFieldAttribute(Type parameterType)
        {
            ParameterType = parameterType;
        }
    }
}
