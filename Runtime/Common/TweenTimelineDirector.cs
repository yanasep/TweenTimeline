using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// TweenTimelineのDirector
    /// TimelineParameterを渡したい場合に使う
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
        [EditorPlayModeButton("Play")]
        public Tween Play()
        {
            return Play(null);
        }
        
        /// <summary>
        /// 再生
        /// </summary>
        public Tween Play(SetParameter setParameter)
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
