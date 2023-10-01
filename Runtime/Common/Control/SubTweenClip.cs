using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// サブTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("SubTween")]
    public class SubTweenClip : TweenClip<PlayableDirector>
    {
        public TimelineAsset timelineAsset;
        
        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<PlayableDirector> info)
        {
            if (timelineAsset == null) return null;
            return TweenTimelineUtility.CreateTween(timelineAsset, info.Parameter, info.Target);
        }
        
        /// <inheritdoc/>
        public override string GetTweenLog(TweenClipInfo<PlayableDirector> info)
        {
            if (timelineAsset == null) return null;
            return TweenTimelineUtility.CreateTweenString(timelineAsset, info.Parameter, info.Target);
        }
    }
}