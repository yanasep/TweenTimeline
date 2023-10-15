namespace TweenTimeline
{
    public abstract class TweenTimelineExpression<T>
    {
        public abstract T Evaluate(TweenParameter parameter);
    }
}