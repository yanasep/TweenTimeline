using System;
using System.Collections.Generic;
using System.Reflection;
using Common;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yanasep;
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
    }
    
    /// <summary>
    /// TweenTimelineのTrackのベースクラス
    /// </summary>
    public abstract class TweenTrack<TBinding> : TweenTrack where TBinding : Object
    {
        // Unityの不具合？でTrackの最初のfoldoutが表示されないっぽいので適当なフィールドで回避
        [SerializeField, ReadOnly] private byte _;
        
        [Space]
        public TweenTimelineFieldOverride[] Overrides;
        public Dictionary<string, TweenTimelineField> Fields { get; set; }

        protected abstract TweenMixerBehaviour<TBinding> Template { get; }

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = (PlayableDirector)graph.GetResolver();
            var binding = director.GetGenericBinding(this) as TBinding;
            if (binding == null) return base.CreateTrackMixer(graph, go, inputCount);
            
            var parameterHolder = director.GetComponent<TweenParameterHolder>();
            var parameter = parameterHolder != null ? parameterHolder.GetParameter() : new TweenParameter();
            
            foreach (var clip in GetClips())
            {
                var tweenClip = clip.asset as TweenClip<TBinding>;
                if (tweenClip != null)
                {
                    tweenClip.StartTime = clip.start;
                    tweenClip.Duration = clip.duration;
                    tweenClip.Binding = binding;
                    tweenClip.Parameter = parameter;
                }
            }

            GatherFields();
            ApplyOverrides(parameter);
            Template.Target = binding;
            return ScriptPlayable<TweenMixerBehaviour>.Create(graph, Template, inputCount);
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

        /// <summary>
        /// TimelineFieldをDictionaryに入れる
        /// </summary>
        private void GatherFields()
        {
            Fields ??= new();
            Fields.Clear();

            // TODO: SourceGeneratorでやる
            var fields = Template.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (!field.FieldType.IsSubclassOf(typeof(TweenTimelineField))) continue;
                Fields.Add(field.Name, (TweenTimelineField)field.GetValue(Template));
            }
        }

        private void ApplyOverrides(TweenParameter parameter)
        {
            if (Overrides == null) return;
            foreach (var fieldOverride in Overrides)
            {
                if (Fields.TryGetValue(fieldOverride.Name, out var field))
                {
                    fieldOverride.Expression.Override(field, parameter);
                }
                // else
                // {
                //     Debug.LogWarning($"{name}: field {fieldOverride.Name} is not found.");
                // }
            }
        }
    }

    /// <summary>
    /// TweenTimelineのMixerBehaviour
    /// </summary>
    public class TweenTrack<TBinding, TMixerBehaviour> : TweenTrack<TBinding> 
        where TBinding : Object
        where TMixerBehaviour : TweenMixerBehaviour<TBinding>
    {
        [SerializeField, ExtractContent] private TMixerBehaviour template;
        protected sealed override TweenMixerBehaviour<TBinding> Template => template;
    }

    /// <summary>
    /// TweenTimelineのMixerBehaviour
    /// </summary>
    public class TweenMixerBehaviour : PlayableBehaviour
    {
        
    }

    /// <summary>
    /// TweenTimelineのMixerBehaviour
    /// </summary>
    public class TweenMixerBehaviour<TBinding> : TweenMixerBehaviour where TBinding : Object
    {
        public TBinding Target { get; set; }
        
        private PlayableDirector director;

        public override void OnPlayableCreate(Playable playable)
        {
            director = (PlayableDirector)playable.GetGraph().GetResolver();
        }

        /// <inheritdoc/>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            OnStart(playable);
        }

        /// <inheritdoc/>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            OnEnd(playable);
        }

        /// <summary>
        /// Track開始時 (ループ時も呼ばれる)
        /// </summary>
        protected virtual void OnStart(Playable playable) { }
        protected virtual void OnEnd(Playable playable) { }
        protected virtual void OnUpdate(Playable playable, double trackTime) { }
        
        /// <inheritdoc/>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
        
            var (jumped, trackTime) = GetWarpedTime(playable, info);
            if (!jumped) return;
            
            // 時間がワープしている場合は、現在時刻の状態を再計算
        
            ResetToOriginalState();
            OnStart(playable);
            int inputCount = playable.GetInputCount();
            
            for (int i = 0; i < inputCount; i++)
            {
                var input = playable.GetInput(i);
                var inputPlayable = (ScriptPlayable<TweenBehaviour>)input;
                var clipBehaviour = inputPlayable.GetBehaviour();
                var clipTime = trackTime - clipBehaviour.StartTime;
                if (clipTime < 0) break;
                clipBehaviour.Start();
                clipBehaviour.Update(Math.Min(clipTime, clipBehaviour.Duration));
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
            OnUpdate(playable, trackTime);
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