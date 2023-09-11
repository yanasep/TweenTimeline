using System;
using System.ComponentModel;
using System.Reflection;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
        
        public abstract TweenMixerBehaviour Template { get; }
    }
    
    /// <summary>
    /// TweenTimelineのTrackのベースクラス
    /// </summary>
    public abstract class TweenTrack<TBinding> : TweenTrack where TBinding : class
    {
        public sealed override TweenMixerBehaviour Template => template;
        protected virtual TweenMixerBehaviour<TBinding> template => null;

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var binding = go.GetComponent<PlayableDirector>().GetGenericBinding(this);

            foreach (var clip in GetClips())
            {
                var animAsset = (TweenClip)clip.asset;
                // TODO
                // animAsset.PlayerData = binding;
                animAsset.Clip = clip;
            }

            // OnPlayableCreateでTargetを参照できるように、Templateにセットしておく
            template.Target = binding as TBinding;
            if (template.Target == null)
            {
                // 何もBindされていなければ、空のビヘイビアを生成
                return base.CreateTrackMixer(graph, go, inputCount);
            }
            var playable = ScriptPlayable<TweenMixerBehaviour>.Create(graph, template, inputCount);
            var behaviour = playable.GetBehaviour();
            if (go.TryGetComponent<TweenParameterInjector>(out var parameterContainer))
            {
                behaviour.Parameter = parameterContainer.GetParameter();
            }
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
    }
    
    /// <summary>
    /// TweenTimelineのMixerBehaviourのベースクラス
    /// </summary>
    public class TweenMixerBehaviour : PlayableBehaviour 
    {
        public TweenParameter Parameter { get; set; }
        public virtual TweenCallback OnStartCallback => null;
        public virtual TweenCallback OnEndCallback => null;
    }

    /// <summary>
    /// TweenTimelineのMixerBehaviourのベースクラス
    /// </summary>
    [Serializable]
    public abstract class TweenMixerBehaviour<TBinding> : TweenMixerBehaviour where TBinding : class
    {
        protected TBinding Target { get; private set; }

        /// <inheritdoc/>
        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
            CacheOriginalState();
        }

        /// <inheritdoc/>
        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if (!Application.isPlaying)
            {
                ResetToOriginalState();
            }
        }

        /// <inheritdoc/>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            OnTrackStart();
        }

        /// <summary>
        /// Track開始時 (ループ時も呼ばれる)
        /// </summary>
        private void OnTrackStart()
        {
            OnStartCallback?.Invoke();
        }

        /// <inheritdoc/>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);

            var (jumped, trackTime) = GetWarpedTime(playable, info);
            if (!jumped) return;
            
            // 時間がワープしている場合は、現在時刻の状態を再計算

            ResetToOriginalState();
            OnTrackStart();
            int inputCount = playable.GetInputCount();
            
            for (int i = 0; i < inputCount; i++)
            {
                // TODO
                // var input = playable.GetInput(i);
                // var inputPlayable = (ScriptPlayable<TweenBehaviour<TBinding>>)input;
                // var clipBehaviour = inputPlayable.GetBehaviour();
                // float clipTime = trackTime - clipBehaviour.StartTime;
                // if (clipTime < 0) break;
                // clipBehaviour.Start(Target);
                // clipBehaviour.Update(Target, Mathf.Min(clipTime, clipBehaviour.Duration));
                // if (clipTime >= clipBehaviour.Duration)
                // {
                //     clipBehaviour.End(Target);
                // }
            }
        }

        /// <summary>
        /// 時刻がワープしたかどうかと、トラックの現在時刻を取得
        /// </summary>
        private (bool warped, float trackTime) GetWarpedTime(Playable playable, FrameData info)
        {
            var time = (float)playable.GetTime();
            if (info.seekOccurred) return (true, time);

            var duration = playable.GetGraph().GetRootPlayable(0).GetDuration();
            var prevTrackTime = playable.GetPreviousTime() % duration;
            var trackTime = playable.GetTime() % duration;
            var warped = prevTrackTime > trackTime;
            return (warped, (float)trackTime);
        }

        /// <summary>
        /// 開始前(プレビュー前)の状態を保存
        /// </summary>
        protected abstract void CacheOriginalState();
        
        /// <summary>
        /// 開始前(プレビュー前)の状態に戻す
        /// </summary>
        protected abstract void ResetToOriginalState();
    }
}