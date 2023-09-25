using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// ネストクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Nest")]
    public class NestClip : TweenClip<TimelineAsset>
    {
        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<TimelineAsset> info)
        {
            return TweenTimelineUtility.CreateTween(info.Target, info.Parameter, 
        }

        /// <inheritdoc/>
        public override string GetTweenLog(TweenClipInfo<TimelineAsset> info)
        {
            return null;
        }
    }
}