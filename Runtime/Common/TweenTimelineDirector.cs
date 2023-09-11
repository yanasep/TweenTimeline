using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
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
        [SerializeField] private TimelineAsset _timelineAsset;
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
            if (_timelineAsset == null) return;
            Play(_timelineAsset);
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
                if (track is TweenTrack tweenTrack)
                {
                    if (tweenTrack.Template.OnStartCallback != null)
                    {
                        sequence.AppendCallback(tweenTrack.Template.OnStartCallback);
                    }
                    float currentTime = 0;
                    foreach (var clip in tweenTrack.GetClips())
                    {
                        var tweenClip = (TweenClip)clip.asset;
                        var template = tweenClip.Template;
                        float interval = (float)clip.start - currentTime;
                        currentTime = (float)(clip.start + clip.duration);
                        if (interval > 0) sequence.AppendInterval(interval);
                        if (template.OnStartCallback != null) sequence.AppendCallback(template.OnStartCallback);
                        var tween = template.GetTween();
                        if (tween != null) sequence.Append(tween);
                        if (template.OnEndCallback != null) sequence.AppendCallback(template.OnEndCallback);
                    }
                    if (tweenTrack.Template.OnEndCallback != null)
                    {
                        sequence.AppendCallback(tweenTrack.Template.OnEndCallback);
                    }   
                }
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
