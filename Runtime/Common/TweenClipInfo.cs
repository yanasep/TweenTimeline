using UnityEngine;

namespace TweenTimeline
{
    public readonly struct TweenClipInfo<T> where T : Object
    {
        public T Target { get; init; }
        public float Duration { get; init; }
        // TODO
        public TweenParameter Parameter { get; init; }
    }
    
    public readonly struct TweenTrackInfo<T> where T : Object
    {
        public T Target { get; init; }
        // TODO
        public TweenParameter Parameter { get; init; }
    }
}
