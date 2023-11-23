using Cysharp.Threading.Tasks;
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
        private Tween _tween;

        public PlayableAsset PlayableAsset => _director.playableAsset;

        public delegate void SetParameter(TweenParameter parameter);

        /// <summary>
        /// 再生
        /// </summary>
        [EditorPlayModeButton("Play", null)]
        public void Play(SetParameter setParameter = null)
        {
            var asset = _director.playableAsset as TimelineAsset;
            if (asset == null) return;
            Play(asset, setParameter);
        }

        /// <summary>
        /// 再生
        /// </summary>
        public void Play(TimelineAsset timelineAsset, SetParameter setParameter = null)
        {
            _tween?.Kill();
            _tween = TweenTimelineUtility.CreateTween(timelineAsset, _director, setParameter);
        }

        /// <summary>
        /// 再生
        /// </summary>
        public UniTask PlayAsync(TimelineAsset timelineAsset, SetParameter setParameter = null)
        {
            Play(timelineAsset, setParameter);
            return _tween.ToUniTask();
        }

        /// <summary>
        /// 再生
        /// </summary>
        public UniTask PlayAsync(SetParameter setParameter = null)
        {
            Play(setParameter);
            return _tween.ToUniTask();
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
