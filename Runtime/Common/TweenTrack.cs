using System;
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
    }
    
    /// <summary>
    /// TweenTimelineのTrackのベースクラス
    /// </summary>
    public abstract class TweenTrack<TBinding> : TweenTrack where TBinding : Object
    {
        public virtual TweenCallback GetStartCallback(TweenTrackInfo<TBinding> info) => null;
        public virtual TweenCallback GetEndCallback(TweenTrackInfo<TBinding> info) => null;
        protected virtual string GetStartLog(TweenTrackInfo<TBinding> info) => null;
        protected virtual string GetEndLog(TweenTrackInfo<TBinding> info) => null;

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
            
            var sequence = DOTween.Sequence().Pause().SetAutoKill(false);
            var tweenTrackInfo = new TweenTrackInfo<TBinding>
            {
                Target = target,
                Parameter = args.Parameter
            };

            var duration = (float)timelineAsset.duration;
            
            var sb = new StringBuilder();
            sb.AppendLine("Sequence()");
            string Indent = "    ";

            var startCallback = GetStartCallback(tweenTrackInfo);
            if (startCallback != null)
            {
                sequence.AppendCallback(startCallback);
                sb.Append(Indent);
                sb.AppendLine(GetStartLog(tweenTrackInfo));
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
                    sb.Append(Indent);
                    sb.AppendLine(tweenClip.GetStartLog(tweenClipInfo));
                }
                // main
                var tween = tweenClip.GetTween(tweenClipInfo);
                if (tween != null)
                {
                    sequence.Append(tween);
                    sb.Append(Indent);
                    sb.AppendLine(tweenClip.GetTweenLog(tweenClipInfo));
                }
                // end
                var clipEndCallback = tweenClip.GetEndCallback(tweenClipInfo);
                if (clipEndCallback != null)
                {
                    sequence.AppendCallback(clipEndCallback);
                    sb.Append(Indent);
                    sb.AppendLine(tweenClip.GetEndLog(tweenClipInfo));
                }
            }

            var endCallback = GetEndCallback(tweenTrackInfo);
            if (endCallback != null)
            {
                float interval = duration - currentTime;
                if (interval > 0) sequence.AppendInterval(interval);
                sequence.AppendCallback(endCallback);
                sb.Append(Indent);
                sb.AppendLine(GetEndLog(tweenTrackInfo));
            }

            sb.Append(Indent);
            sb.Append(";");

            // Debug.Log($"CreateTween ({name}):\n{sb}");
            return sequence;
        }
    }

    /// <summary>
    /// TweenTimelineのMixerBehaviour
    /// </summary>
    public class TweenMixerBehaviour : PlayableBehaviour
    {
        public TweenCallback StartCallback { get; set; }
        public TweenCallback EndCallback { get; set; }

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
            var (jumped, trackTime) = GetWarpedTime(playable, info);
            if (!jumped) return;
            
            // 時間がワープしている場合は、現在時刻の状態を再計算

            OnTrackStart();
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
        private (bool warped, double trackTime) GetWarpedTime(Playable playable, FrameData info)
        {
            var time = playable.GetTime();
            if (info.seekOccurred) return (true, time);

            var duration = playable.GetGraph().GetRootPlayable(0).GetDuration();
            var prevTrackTime = playable.GetPreviousTime() % duration;
            var trackTime = playable.GetTime() % duration;
            var warped = prevTrackTime > trackTime;
            return (warped, trackTime);
        }
    }
}