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
    [ExecuteInEditMode]
    public class TweenTimelineDirector : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _director;

        public TweenParameter Parameter { get; private set; }

        private void OnEnable()
        {
            if (_director == null) return;
            Debug.Log($"onenable");
            
            _director.played += OnPlayed;
        }

        private void OnPlayed(PlayableDirector obj)
        {
            Debug.Log($"{obj.name} played");
        }

        private void OnDisable()
        {
            if (_director == null) return;
            
            _director.played -= OnPlayed;
        }

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
