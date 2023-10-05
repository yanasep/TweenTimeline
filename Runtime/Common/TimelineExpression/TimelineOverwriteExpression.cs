using System;

namespace TweenTimeline
{
    [Serializable]
    public abstract class TimelineOverwriteExpression
    {
        public abstract void Overwrite(TweenTimelineField destination, TweenParameter parameter);
    }
    
    [Serializable]
    public abstract class TimelineOverwriteExpression<T> : TimelineOverwriteExpression
    {
        /// <summary>値取得</summary>
        public abstract T GetValue(TweenParameter parameter, TweenTimelineField field);

        public override void Overwrite(TweenTimelineField destination, TweenParameter parameter)
        {
            ((TweenTimelineField<T>)destination).Value = GetValue(parameter, destination);
        }
    }
}
