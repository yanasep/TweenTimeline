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

        public delegate void SetParameter(TweenParameter parameter);

        // private void OnEnable()
        // {
        //     if (_director == null) return;
        //     Debug.Log($"onenable");
        //     
        //     _director.played += OnPlayed;
        // }
        //
        // private void OnPlayed(PlayableDirector obj)
        // {
        //     Debug.Log($"{obj.name} played");
        // }
        //
        // private void OnDisable()
        // {
        //     if (_director == null) return;
        //     
        //     _director.played -= OnPlayed;
        // }

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
        public void Play(TimelineAsset timelineAsset, SetParameter setParameter = null)
        {
            _tween?.Kill();
            _tween = TweenTimelineUtility.CreateTween(timelineAsset, _director, setParameter);
            _tween.Play();
        }

        /// <summary>
        /// 再生
        /// </summary>
        public UniTask PlayAsync(TimelineAsset timelineAsset, SetParameter setParameter = null)
        {
            Play(timelineAsset, setParameter);
            return _tween.WithCancellation(default);
        }
    }
}
