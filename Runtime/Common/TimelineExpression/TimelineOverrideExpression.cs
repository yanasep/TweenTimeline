using System;

namespace TweenTimeline
{
    [Serializable]
    public abstract class TimelineOverrideExpression
    {
        public abstract void Override(TweenTimelineField destination, TweenParameter parameter);
    }
    
    [Serializable]
    public abstract class TimelineOverrideExpression<T> : TimelineOverrideExpression
    {
        /// <summary>値取得</summary>
        public abstract T GetValue(TweenParameter parameter, TweenTimelineField field);

        public override void Override(TweenTimelineField destination, TweenParameter parameter)
        {
            ((TweenTimelineField<T>)destination).Value = GetValue(parameter, destination);
        }
    }
}
