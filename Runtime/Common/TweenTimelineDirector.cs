using Cysharp.Threading.Tasks;
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
    public class TweenTimelineDirector : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _director;

        public TweenParameter Parameter { get; private set; }

        /// <summary>
        /// 初期化
        /// </summary>
        [EditorPlayModeButton("Initialize")]
        public void Initialize()
        {
            if (TryGetComponent<TweenParameterHolder>(out var holder))
            {
                Parameter = holder.GetParameter();
            }
            else
            {
                Parameter ??= new();
            }
        }

        /// <summary>
        /// 再生
        /// </summary>
        [EditorPlayModeButton("Play")]
        public void Play()
        {
            var asset = _director.playableAsset as TimelineAsset;
            if (asset == null) return;
            Play(asset);
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
            Play(timelineAsset);
            return UniTask.WaitWhile(() => _director.state == PlayState.Playing);
        }
    }
}
