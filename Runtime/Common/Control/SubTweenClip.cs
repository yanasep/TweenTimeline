using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// サブTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("SubTween")]
    public class SubTweenClip : TweenClip<TweenTimelineDirector>
    {
        public TweenTimelineField<TimelineAsset> timelineAsset;

        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<TweenTimelineDirector> info)
        {
            if (timelineAsset == null) return null;
            info.Target.Initialize();
            return info.Target.CreateTween(timelineAsset.Value);
        }
        
        /// <inheritdoc/>
        public override string GetTweenLog(TweenClipInfo<TweenTimelineDirector> info)
        {
            if (timelineAsset == null) return null;
            info.Target.Initialize();
            return info.Target.CreateTweenString(timelineAsset.Value);
        }
    }
}