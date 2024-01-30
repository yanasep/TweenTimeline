namespace TweenTimeline
{
    /// <summary>
    /// 値表現
    /// </summary>
    public abstract class TweenTimelineExpression<T>
    {
        public abstract T Evaluate(TweenParameter parameter);
    }
}
