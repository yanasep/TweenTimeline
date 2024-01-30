using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace TweenTimeline
{
    /// <summary>
    /// タイムラインTweenクリップ
    /// </summary>
    public abstract class TweenClip : PlayableAsset
    {
    }

    /// <summary>
    /// タイムラインTweenクリップ
    /// </summary>
    [Serializable]
    public abstract class TweenClip<TBinding> : TweenClip, ITimelineClipAsset where TBinding : Object
    {
        public virtual ClipCaps clipCaps => ClipCaps.None;

        public abstract Tween CreateTween(TweenClipInfo<TBinding> info);

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return default;
        }
    }

    /// <summary>
    /// タイムラインTweenクリップ
    /// </summary>
    [Serializable]
    public abstract class TweenClipNoBinding : TweenClip, ITimelineClipAsset
    {
        public virtual ClipCaps clipCaps => ClipCaps.None;

        public abstract Tween CreateTween(TweenClipInfo info);

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return default;
        }
    }
}
