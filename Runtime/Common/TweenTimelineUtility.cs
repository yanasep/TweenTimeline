using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    public static class TweenTimelineUtility
    {
        /// <summary>
        /// PlayableAssetをTweenに変換
        /// </summary>
        public static Tween CreateTween(TimelineAsset timelineAsset, TweenParameter parameter, System.Func<TweenTrack, Object> getTrackBinding)
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

        /// <summary>
        /// PlayableAssetをTweenに変換
        /// </summary>
        public static string CreateTweenString(TimelineAsset timelineAsset, TweenParameter parameter, System.Func<TweenTrack, Object> getTrackBinding)
        {
            var sb = new StringBuilder();

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is not TweenTrack tweenTrack) continue;

                sb.AppendLine($"[{tweenTrack.name}]");

                var binding = getTrackBinding(tweenTrack);
                var str = tweenTrack.GetTweenString(new CreateTweenArgs
                {
                    Binding = binding,
                    Parameter = parameter
                });
                if (!string.IsNullOrEmpty(str))
                {
                    sb.AppendLine(str);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}