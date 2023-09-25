using System.Collections.Generic;
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
        private Dictionary<TimelineAsset, Tween> _tweenCache;

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

            _tweenCache = new Dictionary<TimelineAsset, Tween>();
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
            var tween = GetTween(timelineAsset);
            tween.Play();
            return tween;
        }

        private Tween GetTween(TimelineAsset timelineAsset)
        {
            if (!_tweenCache.TryGetValue(timelineAsset, out var tween))
            {
                tween = TweenTimelineUtility.CreateTween(timelineAsset, Parameter, track => _director.GetGenericBinding(track));
                _tweenCache.Add(timelineAsset, tween);
            }

            return tween;
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
            if (_tweenCache == null) return;
            foreach (var tween in _tweenCache.Values)
            {
                tween.Kill();
            }
            _tweenCache.Clear();
        }
    }
}
