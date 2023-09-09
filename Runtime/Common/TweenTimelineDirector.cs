using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// TweenTimelineのDirector
    /// TimelineParameterを渡したい場合に使う
    /// </summary>
    public class TweenTimelineDirector : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _director;
        public TweenParameterContainer ParameterContainer { get; private set; }

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

            if (_director.playableAsset != null)
            {
                SetParameterContainer(_director.playableAsset, ParameterContainer);
            }
        }

        /// <summary>
        /// 再生
        /// </summary>
        public void Play()
        {
            _director.Play();
        }

        /// <summary>
        /// 再生
        /// </summary>
        public void Play(TimelineAsset timelineAsset)
        {
            // TODO: アセットへの変更が残る
            SetParameterContainer(_director.playableAsset, ParameterContainer);
            _director.playableAsset = timelineAsset;
            _director.Play();
        }

        /// <summary>
        /// TimelineParameterContainerをタイムラインに渡す
        /// </summary>
        private void SetParameterContainer(PlayableAsset playableAsset, TweenParameterContainer parameter)
        {
            foreach (var trackBinding in playableAsset.outputs)
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
    }
}
