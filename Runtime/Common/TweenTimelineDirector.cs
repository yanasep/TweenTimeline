using Cysharp.Threading.Tasks;
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
        public TweenParameter Parameter { get; private set; }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            if (TryGetComponent<TweenParameterInjector>(out var injector))
            {
                Parameter = injector.GetParameter();
            }
            else
            {
                Parameter = new();
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
            _director.Play(timelineAsset);
        }

        /// <summary>
        /// 再生
        /// </summary>
        public UniTask PlayAsync(TimelineAsset timelineAsset)
        {
            _director.Play(timelineAsset);
            return UniTask.WaitWhile(() => _director.state == PlayState.Playing);
        }
    }
}
