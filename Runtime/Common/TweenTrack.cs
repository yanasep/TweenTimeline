using System;
using System.ComponentModel;
using System.Reflection;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace TweenTimeline
{
    /// <summary>
    /// TweenTimelineのTrackのベースクラス
    /// </summary>
    public abstract class TweenTrack : TrackAsset 
    {
#if UNITY_EDITOR
        /// <summary>Trackのアイコン</summary>
        /// <remarks>ビルトインアイコン： https://github.com/halak/unity-editor-icons</remarks>
        public virtual Texture2D Icon => null;  
#endif

        public abstract Tween CreateTween(Object binding);
    }
    
    /// <summary>
    /// TweenTimelineのTrackのベースクラス
    /// </summary>
    public abstract class TweenTrack<TBinding> : TweenTrack where TBinding : Object
    {
        protected virtual TweenCallback GetStartCallback(TweenTrackInfo<TBinding> info) => null;
        protected virtual TweenCallback GetEndCallback(TweenTrackInfo<TBinding> info) => null;

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var binding = go.GetComponent<PlayableDirector>().GetGenericBinding(this);

            // OnPlayableCreateで参照できるように、Templateにセットしておく
            var tween = CreateTween(binding);
            if (tween == null)
            {
                // Tweenがなければ、空のビヘイビアを生成
                return base.CreateTrackMixer(graph, go, inputCount);
            }
            
            var playable = ScriptPlayable<TweenMixerBehaviour<TBinding>>.Create(graph, inputCount);
            var behaviour = playable.GetBehaviour();
            behaviour.Tween = tween;
            return playable;
        }

        /// <inheritdoc/>
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
#if UNITY_EDITOR
            var attr = clip.asset.GetType().GetCustomAttribute<DisplayNameAttribute>(inherit: true);
            if (attr != null)
            {
                clip.displayName = attr.DisplayName;
            }      
#endif
        }

        public sealed override Tween CreateTween(Object binding)
        {
            var target = binding as TBinding;
            if (target == null) return null;
            
            var sequence = DOTween.Sequence().Pause().SetAutoKill(false);
            var tweenTrackInfo = new TweenTrackInfo<TBinding>
            {
                Target = target,
                Parameter = null
            };

            var startCallback = GetStartCallback(tweenTrackInfo);
            if (startCallback != null) sequence.AppendCallback(startCallback);

            float currentTime = 0;
            foreach (var clip in GetClips())
            {
                // interval
                var tweenClip = (TweenClip<TBinding>)clip.asset;
                float interval = (float)clip.start - currentTime;
                currentTime = (float)(clip.start + clip.duration);
                if (interval > 0) sequence.AppendInterval(interval);

                var tweenClipInfo = new TweenClipInfo<TBinding>
                {
                    Target = target,
                    Duration = (float)clip.duration,
                    Parameter = null
                };
                // start
                var clipStartCallback = tweenClip.GetStartCallback(tweenClipInfo);
                if (clipStartCallback != null) sequence.AppendCallback(clipStartCallback);
                // main
                var tween = tweenClip.GetTween(tweenClipInfo);
                if (tween != null) sequence.Append(tween);
                // end
                var clipEndCallback = tweenClip.GetEndCallback(tweenClipInfo);
                if (clipEndCallback != null) sequence.AppendCallback(clipEndCallback);
            }

            var endCallback = GetEndCallback(tweenTrackInfo);
            if (endCallback != null) sequence.AppendCallback(endCallback);

            return sequence;
        }
    }
    
    /// <summary>
    /// TweenTimelineのMixerBehaviourのベースクラス
    /// </summary>
    public class TweenMixerBehaviour : PlayableBehaviour 
    {
    }

    /// <summary>
    /// TweenTimelineのMixerBehaviourのベースクラス
    /// </summary>
    [Serializable]
    public class TweenMixerBehaviour<TBinding> : TweenMixerBehaviour where TBinding : Object
    {
        public Tween Tween { get; set; }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            Debug.Log($"tl prepare");
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Debug.Log($"tl process frame");
            var duration = playable.GetGraph().GetRootPlayable(0).GetDuration();
            var trackTime = playable.GetTime() % duration;
            Tween.Goto((float)trackTime);
        }

        // /// <summary>
        // /// Track開始時 (ループ時も呼ばれる)
        // /// </summary>
        // private void OnTrackStart()
        // {
        //     OnStartCallback?.Invoke();
        // }

        // /// <inheritdoc/>
        // public override void PrepareFrame(Playable playable, FrameData info)
        // {
        //     base.PrepareFrame(playable, info);
        //
        //     var (jumped, trackTime) = GetWarpedTime(playable, info);
        //     if (!jumped) return;
        //     
        //     // 時間がワープしている場合は、現在時刻の状態を再計算
        //
        //     ResetToOriginalState();
        //     OnTrackStart();
        //     int inputCount = playable.GetInputCount();
        //     
        //     for (int i = 0; i < inputCount; i++)
        //     {
        //         var input = playable.GetInput(i);
        //         var inputPlayable = (ScriptPlayable<TweenBehaviour>)input;
        //         var clipBehaviour = inputPlayable.GetBehaviour();
        //         float clipTime = trackTime - clipBehaviour.StartTime;
        //         if (clipTime < 0) break;
        //         clipBehaviour.Start();
        //         clipBehaviour.Update(Mathf.Min(clipTime, clipBehaviour.Duration));
        //         if (clipTime >= clipBehaviour.Duration)
        //         {
        //             clipBehaviour.End();
        //         }
        //     }
        // }
        //
        // /// <summary>
        // /// 時刻がワープしたかどうかと、トラックの現在時刻を取得
        // /// </summary>
        // private (bool warped, float trackTime) GetWarpedTime(Playable playable, FrameData info)
        // {
        //     var time = (float)playable.GetTime();
        //     if (info.seekOccurred) return (true, time);
        //
        //     var duration = playable.GetGraph().GetRootPlayable(0).GetDuration();
        //     var prevTrackTime = playable.GetPreviousTime() % duration;
        //     var trackTime = playable.GetTime() % duration;
        //     var warped = prevTrackTime > trackTime;
        //     return (warped, (float)trackTime);
        // }

        /// <summary>
        /// 開始前(プレビュー前)の状態を保存
        /// </summary>
        protected virtual void CacheOriginalState(){}
        
        /// <summary>
        /// 開始前(プレビュー前)の状態に戻す
        /// </summary>
        protected virtual void ResetToOriginalState(){}
    }
}