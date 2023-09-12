using System.Collections.Generic;
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
            if (TryGetComponent<TweenParameterInjector>(out var injector))
            {
                Parameter = injector.GetParameter();
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
            Play(asset);
        }

        /// <summary>
        /// 再生
        /// </summary>
        public void Play(TimelineAsset timelineAsset)
        {
            var tween = GetTween(timelineAsset);
            tween.Play();
        }

        /// <summary>
        /// 再生
        /// </summary>
        public UniTask PlayAsync(TimelineAsset timelineAsset)
        {
            var tween = GetTween(timelineAsset);
            tween.Play();
            return tween.ToUniTask();
        }

        private Tween GetTween(TimelineAsset timelineAsset)
        {
            if (!_tweenCache.TryGetValue(timelineAsset, out var tween))
            {
                tween = CreateTween(timelineAsset);
                _tweenCache.Add(timelineAsset, tween);
            }

            return tween;
        }
        
        /// <summary>
        /// PlayableAssetをTweenに変換
        /// </summary>
        private Tween CreateTween(TimelineAsset timelineAsset)
        {
            var sequence = DOTween.Sequence().Pause().SetAutoKill(false);
            
            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is not TweenTrack tweenTrack) continue;

                var binding = _director.GetGenericBinding(track);
                var tween = tweenTrack.CreateTween(binding);
                if (tween != null) sequence.Join(tween);
            }
        
            return sequence;
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
