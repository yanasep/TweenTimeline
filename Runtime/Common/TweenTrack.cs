using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
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
        public abstract string GetTweenString(CreateTweenArgs args);
    }
    
    /// <summary>
    /// TweenTimelineのTrackのベースクラス
    /// </summary>
    public abstract class TweenTrack<TBinding> : TweenTrack where TBinding : Object
    {
        protected virtual TweenCallback GetStartCallback(TweenTrackInfo<TBinding> info) => null;
        protected virtual TweenCallback GetEndCallback(TweenTrackInfo<TBinding> info) => null;
        protected virtual string GetStartLog(TweenTrackInfo<TBinding> info) => null;
        protected virtual string GetEndLog(TweenTrackInfo<TBinding> info) => null;

        // Unityの不具合？でTrackの最初のfoldoutが表示されないっぽいので適当なフィールドで回避
        [SerializeField, Common.ReadOnly] private byte _;
        
        [Space]
        public TweenTimelineFieldOverride[] Overrides;
        public Dictionary<string, TweenTimelineField> Fields { get; set; }

        private readonly TweenMixerBehaviour template = new();

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        { 
            var binding = go.GetComponent<PlayableDirector>().GetGenericBinding(this) as TBinding;
            if (binding == null) return base.CreateTrackMixer(graph, go, inputCount);
            
            var parameterHolder = go.GetComponent<TweenParameterHolder>();
            var parameter = parameterHolder != null ? parameterHolder.GetParameter() : new TweenParameter();
            
            foreach (var clip in GetClips())
            {
                var tweenClip = clip.asset as TweenClip<TBinding>;
                if (tweenClip != null)
                {
                    tweenClip.StartTime = clip.start;
                    tweenClip.Duration = clip.duration;
                    tweenClip.Binding = binding;
                }
            }

            var trackInfo = new TweenTrackInfo<TBinding>
            {
                Target = binding,
                Parameter = parameter
            };
            template.Tween = CreateTween(new CreateTweenArgs
            {
                Binding = binding,
                Parameter = parameter
            });
            template.StartCallback = GetStartCallback(trackInfo);
            template.EndCallback = GetEndCallback(trackInfo);
            return ScriptPlayable<TweenMixerBehaviour>.Create(graph, template, inputCount);
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

        public sealed override Tween CreateTween(CreateTweenArgs args)
        {
            var target = args.Binding as TBinding;
            if (target == null) return null;

            GatherFields();
            ApplyOverrides(args.Parameter);
            
            var sequence = DOTween.Sequence().Pause().SetAutoKill(false);
            var tweenTrackInfo = new TweenTrackInfo<TBinding>
            {
                Target = target,
                Parameter = args.Parameter
            };

            var duration = (float)timelineAsset.duration;

            var startCallback = GetStartCallback(tweenTrackInfo);
            if (startCallback != null)
            {
                sequence.AppendCallback(startCallback);
            }

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
                    Parameter = args.Parameter
                };
                // start
                var clipStartCallback = tweenClip.GetStartCallback(tweenClipInfo);
                if (clipStartCallback != null)
                {
                    sequence.AppendCallback(clipStartCallback);
                }
                // main
                var tween = tweenClip.CreateTween(tweenClipInfo);
                if (tween != null)
                {
                    sequence.Append(tween);
                }
                // end
                var clipEndCallback = tweenClip.GetEndCallback(tweenClipInfo);
                if (clipEndCallback != null)
                {
                    sequence.AppendCallback(clipEndCallback);
                }
            }

            var endCallback = GetEndCallback(tweenTrackInfo);
            if (endCallback != null)
            {
                float interval = duration - currentTime;
                if (interval > 0) sequence.AppendInterval(interval);
                sequence.AppendCallback(endCallback);
            }

            return sequence;
        }

        public sealed override string GetTweenString(CreateTweenArgs args)
        {
            var target = args.Binding as TBinding;
            if (target == null) return null;
            
            var tweenTrackInfo = new TweenTrackInfo<TBinding>
            {
                Target = target,
                Parameter = args.Parameter
            };

            var duration = (float)timelineAsset.duration;
            
            var sb = new StringBuilder();
            sb.Append("Sequence()");
            string Prefix = $"{Environment.NewLine}    ";

            void AppendIntervalLog(float interval)
            {
                if (interval > 0)
                {
                    sb.Append(Prefix);
                    sb.Append($"Interval {interval}s");
                }
            }
            
            void AppendObjectLog(object obj, string log, string callbackName)
            {
                if (obj == null) return;
                sb.Append(Prefix);
                if (string.IsNullOrEmpty(log)) log = $"Log Not Implemented {callbackName}";
                sb.Append(log);
            }

            AppendObjectLog(GetStartCallback(tweenTrackInfo), GetStartLog(tweenTrackInfo), "TrackStartCallback");

            float currentTime = 0;
            foreach (var clip in GetClips())
            {
                // interval
                var tweenClip = (TweenClip<TBinding>)clip.asset;
                float interval = (float)clip.start - currentTime;
                currentTime = (float)(clip.start + clip.duration);
                AppendIntervalLog(interval);

                var tweenClipInfo = new TweenClipInfo<TBinding>
                {
                    Target = target,
                    Duration = (float)clip.duration,
                    Parameter = args.Parameter
                };
                // start
                AppendObjectLog(tweenClip.GetStartCallback(tweenClipInfo), tweenClip.GetStartLog(tweenClipInfo), "ClipStartCallback");
                // main
                AppendObjectLog(tweenClip.CreateTween(tweenClipInfo), tweenClip.GetTweenLog(tweenClipInfo), "ClipTween");
                // end
                AppendObjectLog(tweenClip.GetEndCallback(tweenClipInfo), tweenClip.GetEndLog(tweenClipInfo), "ClipEndCallback");
            }

            var endCallback = GetEndCallback(tweenTrackInfo);
            if (endCallback != null)
            {
                float interval = duration - currentTime;
                AppendIntervalLog(interval);
                AppendObjectLog(endCallback, GetEndLog(tweenTrackInfo), "TrackEndCallback");
            }

            sb.Append(";");

            return sb.ToString();
        }

        /// <summary>
        /// TimelineFieldをDictionaryに入れる
        /// </summary>
        private void GatherFields()
        {
            Fields ??= new();
            Fields.Clear();

            // TODO: SourceGeneratorでやる
            var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (!field.FieldType.IsSubclassOf(typeof(TweenTimelineField))) continue;
                Fields.Add(field.Name, (TweenTimelineField)field.GetValue(this));
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
                else
                {
                    Debug.LogWarning($"{name}: field {fieldOverride.Name} is not found.");
                }
            }
        }
    }

    /// <summary>
    /// TweenTimelineのMixerBehaviour
    /// </summary>
    public class TweenMixerBehaviour : PlayableBehaviour
    {
        public Tween Tween { get; set; }
        public TweenCallback StartCallback { get; set; }
        public TweenCallback EndCallback { get; set; }

        private PlayableDirector director;

        public override void OnPlayableCreate(Playable playable)
        {
            director = (PlayableDirector)playable.GetGraph().GetResolver();
        }

        /// <inheritdoc/>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            OnTrackStart();
        }

        /// <inheritdoc/>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            EndCallback?.Invoke();
        }

        /// <summary>
        /// Track開始時 (ループ時も呼ばれる)
        /// </summary>
        private void OnTrackStart()
        {
            StartCallback?.Invoke();
        }

        /// <inheritdoc/>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            var trackTime = (float)GetTrackTime(playable.GetTime(), playable.GetGraph().GetRootPlayable(0).GetDuration()); 
            Tween.GotoWithCallbacks(trackTime);
        }
        
        private double GetTrackTime(double time, double duration)
        {
            return director.extrapolationMode switch
            {
                DirectorWrapMode.Hold => Math.Min(time, duration),
                _ => time % duration
            };
        }
    }
}