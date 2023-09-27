using System.Text;
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
    public class TweenTimelineDirector : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _director;

        public TweenParameter Parameter { get; private set; }
        private Tween _tween;

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
                Parameter = new();
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
            PlayCore(asset);
        }

        /// <summary>
        /// 再生
        /// </summary>
        public void Play(TimelineAsset timelineAsset)
        {
            PlayCore(timelineAsset);
        }

        /// <summary>
        /// 再生
        /// </summary>
        public UniTask PlayAsync(TimelineAsset timelineAsset)
        {
            return PlayCore(timelineAsset).ToUniTask();
        }

        private Tween PlayCore(TimelineAsset timelineAsset)
        {
            _tween?.Kill();
            _tween = TweenTimelineUtility.CreateTween(timelineAsset, Parameter, track => _director.GetGenericBinding(track));
            _tween.Play();
            return _tween;
        }

        [EditorButton("LogTween")]
        public void LogTween()
        {
            var asset = _director.playableAsset as TimelineAsset;
            if (asset == null)
            {
                Debug.Log($"PlayableAsset is null");
                return;
            }

            Debug.Log("Tween Log\n" + CreateTweenString(asset));
        }

        /// <summary>
        /// PlayableAssetをTweenに変換
        /// </summary>
        private string CreateTweenString(TimelineAsset timelineAsset)
        {
            var sb = new StringBuilder();

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is not TweenTrack tweenTrack) continue;

                sb.AppendLine($"[{tweenTrack.name}]");

                var binding = _director.GetGenericBinding(track);
                var str = tweenTrack.GetTweenString(new CreateTweenArgs
                {
                    Binding = binding,
                    Parameter = Parameter
                });
                if (!string.IsNullOrEmpty(str))
                {
                    sb.AppendLine(str);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}
