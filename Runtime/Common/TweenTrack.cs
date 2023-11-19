using System;
using System.Collections.Generic;
using System.Reflection;
using Common;
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
        
        public abstract Tween CreateTween(CreateTweenArgs args);
    }
    
    /// <summary>
    /// TweenTimelineのTrackのベースクラス
    /// </summary>
    public abstract class TweenTrack<TBinding> : TweenTrack where TBinding : Object
    {
        // Unityの不具合？でTrackの最初のfoldoutが表示されないっぽいので適当なフィールドで回避
        [SerializeField, ReadOnly] private byte _;
        
        public override Tween CreateTween(CreateTweenArgs args)
        {
            var target = args.Binding as TBinding;
            if (target == null) return null;
            
            var sequence = DOTween.Sequence().Pause().SetAutoKill(false);

            float currentTime = 0;
            foreach (var clip in GetClips())
            {
                // interval
                var tweenClip = (TweenClip<TBinding>)clip.asset;
                tweenClip.target = target;
                float interval = (float)clip.start - currentTime;
                currentTime = (float)(clip.start + clip.duration);
                if (interval > 0) sequence.AppendInterval(interval);

                var tweenClipInfo = new TweenClipInfo<TBinding>
                {
                    Target = target,
                    Duration = (float)clip.duration,
                    Parameter = args.Parameter
                };
                var tween = tweenClip.CreateTween(tweenClipInfo);
                if (tween != null)
                {
                    sequence.Append(tween);
                }
            }

            var remain = duration - currentTime;
            if (remain > 0)
            {
                sequence.AppendInterval((float)remain);
            }

            return sequence;
        }

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = (PlayableDirector)graph.GetResolver();
            var binding = director.GetGenericBinding(this) as TBinding;
            if (binding == null) return base.CreateTrackMixer(graph, go, inputCount);

            var parameter = TweenTimelineUtility.GetTweenParameter(timelineAsset);
            var playable = ScriptPlayable<TweenMixerBehaviour>.Create(graph, inputCount);
            playable.GetBehaviour().Tween = CreateTween(new CreateTweenArgs
            {
                Binding = binding,
                Parameter = parameter,
                Duration = (float)timelineAsset.duration
            });
            return playable;
        }
        
        /// <inheritdoc/>
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
#if UNITY_EDITOR
            var attr = clip.asset.GetType().GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>(inherit: true);
            if (attr != null)
            {
                clip.displayName = attr.DisplayName;
            }      
#endif
        }
        
        public readonly struct ClipInputs
        {
            public readonly List<(float start, float end)> ClipIntervals;

            public ClipInputs(List<(float start, float end)> clipIntervals)
            {
                ClipIntervals = clipIntervals;
            }

            public bool IsAnyPlaying(float trackTime)
            {
                bool active = false;
                for (int i = 0; i < ClipIntervals.Count; i++)
                {
                    if (ClipIntervals[i].start <= trackTime && trackTime <= ClipIntervals[i].end)
                    {
                        active = true;
                        break;
                    }
                }

                return active;
            }

            public bool HasAnyStarted(float trackTime)
            {
                bool active = false;
                for (int i = 0; i < ClipIntervals.Count; i++)
                {
                    if (ClipIntervals[i].start <= trackTime)
                    {
                        active = true;
                        break;
                    }
                }

                return active;
            }
        }

        public ClipInputs GetClipInputs()
        {
            var inputs = new ClipInputs(new());

            foreach (var clip in GetClips())
            {
                inputs.ClipIntervals.Add(((float)clip.start, (float)(clip.start + clip.duration)));
            }

            return inputs;
        }
    }
    
    /// <summary>
    /// TweenTimelineのMixerBehaviour
    /// </summary>
    public class TweenMixerBehaviour : PlayableBehaviour
    {
        public Tween Tween { get; set; }
        private PlayableDirector director;

        public override void OnPlayableCreate(Playable playable)
        {
            director = playable.GetGraph().GetResolver() as PlayableDirector;
        }

        /// <inheritdoc/>
        public sealed override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            OnStart(playable);
        }

        /// <inheritdoc/>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);
            OnEnd(playable);
        }

        /// <summary>
        /// Track開始時 (ループ時も呼ばれる)
        /// </summary>
        protected virtual void OnStart(Playable playable) { }
        protected virtual void OnEnd(Playable playable) { }
        protected virtual void OnUpdate(Playable playable, double trackTime) { }
        
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
        //     OnStart(playable);
        //     int inputCount = playable.GetInputCount();
        //     
        //     for (int i = 0; i < inputCount; i++)
        //     {
        //         var input = playable.GetInput(i);
        //         var inputPlayable = (ScriptPlayable<TweenBehaviour>)input;
        //         var clipBehaviour = inputPlayable.GetBehaviour();
        //         var clipTime = trackTime - clipBehaviour.StartTime;
        //         if (clipTime < 0) break;
        //         clipBehaviour.Start();
        //         clipBehaviour.Update(Math.Min(clipTime, clipBehaviour.Duration));
        //         if (clipTime >= clipBehaviour.Duration)
        //         {
        //             clipBehaviour.End();
        //         }
        //     }
        // }
        
        /// <summary>
        /// 時刻がワープしたかどうかと、トラックの現在時刻を取得
        /// </summary>
        private (bool warped, float trackTime) GetWarpedTime(Playable playable, FrameData info)
        {
            var time = (float)playable.GetTime();
            if (info.seekOccurred) return (true, time);
        
            var duration = playable.GetGraph().GetRootPlayable(0).GetDuration();
            var prevTrackTime = GetTrackTime(playable.GetPreviousTime(), duration);
            var trackTime = GetTrackTime(playable.GetTime(), duration);
            var warped = prevTrackTime > trackTime;
            // var warped = info.evaluationType == FrameData.EvaluationType.Evaluate;
            return (warped, (float)trackTime);
        }

        /// <inheritdoc/>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {   
            var trackTime = (float)GetTrackTime(playable.GetTime(), playable.GetGraph().GetRootPlayable(0).GetDuration());
            // OnUpdate(playable, trackTime);
            
            // var duration = playable.GetGraph().GetRootPlayable(0).GetDuration();
            // var trackTime = GetTrackTime(playable.GetTime(), duration);
            Tween.GotoWithCallbacks(trackTime);
        }

        private double GetTrackTime(double time, double duration)
        {
            return director.extrapolationMode switch
            {
                DirectorWrapMode.Loop => time % duration,
                _ => Math.Min(time, duration)
            };
        }

        private void ResetToOriginalState()
        {
            // TODO
        }

        public bool IsAnyClipPlaying(Playable playable)
        {
            int inputCount = playable.GetInputCount();
            bool hasInput = false;
            for (int i = 0; i < inputCount; i++)
            {
                if (playable.GetInputWeight(i) > 0)
                {
                    hasInput = true;
                    break;
                }
            }

            return hasInput;
        }

        public bool HasAnyClipStarted(Playable playable)
        {   
            int inputCount = playable.GetInputCount();
            bool hasInput = false;
            for (int i = 0; i < inputCount; i++)
            {
                var time = playable.GetInput(i).GetTime();
                Debug.Log($"input time {time}, index {i}");
                if (playable.GetInputWeight(i) > 0)
                {
                    hasInput = true;
                    break;
                }
            }

            return hasInput;
        }
    }
}