using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// TweenTimelineのDirector (TweenTimelineは通常のPlayableDirectorだと再生できない)
    /// </summary>
    [ExecuteInEditMode]
    public class TweenTimelineDirector : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _director;

        public PlayableAsset PlayableAsset => _director.playableAsset;

        public delegate void SetParameter(TweenParameter parameter);

        /// <summary>
        /// 再生
        /// </summary>
        public Tween Play(SetParameter setParameter = null)
        {
            var asset = _director.playableAsset as TimelineAsset;
            if (asset == null) return null;
            return Play(asset, setParameter);
        }

        /// <summary>
        /// 再生
        /// </summary>
        public Tween Play(TimelineAsset timelineAsset, SetParameter setParameter = null)
        {
            var tween = TweenTimelineUtility.CreateTween(timelineAsset, _director, setParameter);
            return tween;
        }

        /// <summary>
        /// Trackの対象オブジェクトをセット
        /// </summary>
        public void SetTrackBinding(PlayableAsset timelineAsset, string trackName, Object value)
        {
            foreach (var trackBinding in timelineAsset.outputs)
            {
                if (trackBinding.streamName == trackName)
                {
                    _director.SetGenericBinding(trackBinding.sourceObject, value);
                    break;
                }
            }
        }

        /// <summary>
        /// Trackの対象オブジェクトを取得
        /// </summary>
        public Object GetTrackBinding(PlayableAsset timelineAsset, string trackName)
        {
            foreach (var trackBinding in timelineAsset.outputs)
            {
                if (trackBinding.streamName == trackName)
                {
                    return _director.GetGenericBinding(trackBinding.sourceObject);
                }
            }

            return null;
        }
    }
}
