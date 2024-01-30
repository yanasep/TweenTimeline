using System;

namespace TweenTimeline
{
    [Serializable]
    public class TimelineParameterReference<T> where T : class
    {
        public uint ParameterId;

        public T Evaluate(TweenParameter parameter)
        {
            return parameter.GetObject(ParameterId) as T;
        }
    }
}
