using UnityEngine;
using UnityEngine.Playables;

namespace TweenTimeline
{
    /// <summary>
    /// TweenTimelineのDirector
    /// TimelineParameterを渡したい場合に使う
    /// </summary>
    public class TweenTimelineDirector : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _director;
        public TimelineParameterContainer ParameterContainer { get; private set; }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            if (TryGetComponent<TweenTimelineDefaultParameter>(out var defaultComponent))
            {
                ParameterContainer = defaultComponent.GetParameterContainer();
            }
            else
            {
                ParameterContainer = new();
            }

            SetParameterContainer(ParameterContainer);
        }

        /// <summary>
        /// TimelineParameterContainerをタイムラインに渡す
        /// </summary>
        private void SetParameterContainer(TimelineParameterContainer parameter)
        {
            foreach (var trackBinding in _director.playableAsset.outputs)
            {
                if (trackBinding.sourceObject is TweenTrack tweenTrack)
                {
                    tweenTrack.Parameter = parameter;
                    foreach (var clip in tweenTrack.GetClips())
                    {
                        var tweenClip = (TweenClip)clip.asset;
                        tweenClip.Parameter = parameter;
                    }   
                }
            }
        }

        /// <summary>
        /// 再生
        /// </summary>
        public void Play()
        {
            _director.Play();
        }
    }
}
