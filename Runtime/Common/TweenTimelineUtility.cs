using System;
using DG.Tweening;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace TweenTimeline
{
    public static class TweenTimelineUtility
    {
        /// <summary>
        /// PlayableAssetをTweenに変換
        /// </summary>
        public static Tween CreateTween(TimelineAsset timelineAsset, TweenParameter parameter, Func<TweenTrack, Object> getTrackBinding)
        {
            var sequence = DOTween.Sequence().Pause().SetAutoKill(false);

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is not TweenTrack tweenTrack) continue;

                var binding = getTrackBinding(tweenTrack);
                var tween = tweenTrack.CreateTween(new CreateTweenArgs
                {
                    Binding = binding,
                    Parameter = parameter
                });
                if (tween != null) sequence.Join(tween);
            }

            return sequence;
        }
    }
}