﻿using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// ネストクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Nest")]
    public class SubTweenClip : TweenClip<TimelineAsset>
    {
        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<TimelineAsset> info)
        {
            throw new NotImplementedException();
            // return TweenTimelineUtility.CreateTween(info.Target, info.Parameter, 
        }

        /// <inheritdoc/>
        public override string GetTweenLog(TweenClipInfo<TimelineAsset> info)
        {
            return null;
        }
    }
}