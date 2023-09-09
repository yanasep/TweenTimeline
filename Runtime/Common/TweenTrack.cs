using System;
using System.ComponentModel;
using System.Reflection;
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
        
        public TweenParameterContainer Parameter { get; set; }
    }
    
    /// <summary>
    /// TweenTimelineのTrackのベースクラス
    /// </summary>
    public abstract class TweenTrack<TBinding> : TweenTrack where TBinding : class
    {
        protected virtual TweenMixerBehaviour<TBinding> Template => null;

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var binding = go.GetComponent<PlayableDirector>().GetGenericBinding(this);

            foreach (var clip in GetClips())
            {
                var animAsset = (TweenClip)clip.asset;
                animAsset.PlayerData = binding;
                animAsset.Clip = clip;
            }

            // OnPlayableCreateでTargetを参照できるように、Templateにセットしておく
            Template.Target = binding as TBinding;
            if (Template.Target == null)
            {
                // 何もBindされていなければ、空のビヘイビアを生成
                return base.CreateTrackMixer(graph, go, inputCount);
            }
            var playable = ScriptPlayable<TweenMixerBehaviour>.Create(graph, Template, inputCount);
            var behaviour = playable.GetBehaviour();
            behaviour.Parameter = Parameter;
            // デフォルトのパラメータを注入
            if (behaviour.Parameter == null)
            {
                if (go.TryGetComponent<TweenTimelineDefaultParameter>(out var parameterComponent))
                {
                    behaviour.Parameter = parameterComponent.GetParameterContainer();
                }
                else
                {
                    behaviour.Parameter = new TweenParameterContainer();
                }
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
        public TweenParameterContainer Parameter { get; set; }
    }

    /// <summary>
    /// TweenTimelineのMixerBehaviourのベースクラス
    /// </summary>
    [Serializable]
    public abstract class TweenMixerBehaviour<TBinding> : TweenMixerBehaviour
    {
        public TBinding Target { get; set; }

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
        protected virtual void OnTrackStart() { }

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
                var input = playable.GetInput(i);
                var inputPlayable = (ScriptPlayable<TweenBehaviour>)input;
                var clipBehaviour = inputPlayable.GetBehaviour();
                float clipTime = trackTime - clipBehaviour.StartTime;
                if (clipTime < 0) break;
                clipBehaviour.Start();
                clipBehaviour.Update(Mathf.Min(clipTime, clipBehaviour.Duration));
                if (clipTime >= clipBehaviour.Duration)
                {
                    clipBehaviour.End();
                }
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