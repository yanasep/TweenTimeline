using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// TimelineのUtility
    /// </summary>
    public static class TweenTimelineUtility
    {
        /// <summary>
        /// PlayableAssetをTweenに変換
        /// </summary>
        public static Tween CreateTween(TimelineAsset timelineAsset, PlayableDirector director, 
            TweenTimelineDirector.SetParameter setParameter)
        {
            var sequence = DOTween.Sequence();

            var parameter = GetTweenParameter(timelineAsset);
            setParameter?.Invoke(parameter);

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track.mutedInHierarchy) continue;
                if (track is not TweenTrack tweenTrack) continue;

                var binding = director.GetGenericBinding(tweenTrack);
                var tween = tweenTrack.CreateTween(new CreateTweenArgs
                {
                    Binding = binding,
                    Parameter = parameter,
                    Duration = (float)timelineAsset.duration
                });
                if (tween != null) sequence.Join(tween);
            }

            return sequence;
        }

        public static TweenParameter GetTweenParameter(TimelineAsset timelineAsset)
        {
            var track = FindTweenParameterTrack(timelineAsset);
            return track == null ? null : track.GetParameter();
        }

        public static TweenParameterTrack FindTweenParameterTrack(TimelineAsset timelineAsset)
        {
            if (timelineAsset == null) return null;

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track.mutedInHierarchy) continue;
                if (track is TweenParameterTrack parameterTrack)
                {
                    return parameterTrack;
                }
            }

            return null;
        }
    }
}