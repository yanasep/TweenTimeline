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

        // private readonly TweenMixerBehaviour<TBinding> template = new();
        public virtual TweenMixerBehaviour<TBinding> Template { get; }// = new();

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        { 
            var binding = go.GetComponent<PlayableDirector>().GetGenericBinding(this);
            var parameterHolder = go.GetComponent<TweenParameterHolder>();
            var parameter = parameterHolder != null ? parameterHolder.GetParameter() : new TweenParameter();

            // var tween = CreateTween(new CreateTweenArgs
            // {
            //     Binding = binding,
            //     Parameter = parameter
            // });
            // if (tween == null)
            // {
            //     // Tweenがなければ、空のビヘイビアを生成
            //     return base.CreateTrackMixer(graph, go, inputCount);
            // }
            // var playable = ScriptPlayable<TweenMixerBehaviour>.Create(graph, inputCount);
            // var behaviour = playable.GetBehaviour();
            // behaviour.Tween = tween;
            // return playable;
            
            var bindingT = binding as TBinding;
            if (bindingT == null) return base.CreateTrackMixer(graph, go, inputCount);

            foreach (var clip in GetClips())
            {
                var tweenClip = clip.asset as TweenClip<TBinding>;
                if (tweenClip != null)
                {
                    tweenClip.StartTime = clip.start;
                    tweenClip.Binding = bindingT;
                }
            }

            TweenMixerBehaviour<TBinding> template = Template ?? new TweenMixerBehaviour<TBinding>();
            template.Track = this;
            template.TrackInfo = new TweenTrackInfo<TBinding>
            {
                Target = bindingT,
                Parameter = parameter
            };
            return ScriptPlayable<TweenMixerBehaviour<TBinding>>.Create(graph, template, inputCount);
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
    /// TweenTimelineのMixerBehaviourのベースクラス
    /// </summary>
    public class TweenMixerBehaviour : PlayableBehaviour 
    {
        // public Tween Tween { get; set; }
        //
        // /// <inheritdoc/>
        // public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        // {
        //     var trackTime = playable.GetTime();
        //
        //     var graph = playable.GetGraph();
        //     var director = (PlayableDirector)graph.GetResolver();
        //     if (director.extrapolationMode == DirectorWrapMode.Loop)
        //     {
        //         var duration = graph.GetRootPlayable(0).GetDuration();
        //         trackTime = playable.GetTime() % duration;
        //     }
        //
        //     Tween.GotoWithCallbacks((float)trackTime);
        // }
        //
        // /// <inheritdoc/>
        // public override void OnPlayableDestroy(Playable playable)
        // {
        //     Tween.Rewind();
        // }
    }

    /// <summary>
    /// TweenTimelineのMixerBehaviourのベースクラス
    /// </summary>
    public class TweenMixerBehaviour<TBinding> : PlayableBehaviour where TBinding : Object
    {
        public TweenTrack<TBinding> Track { get; set; }
        public TweenTrackInfo<TBinding> TrackInfo { get; set; }
        
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
            Track.GetStartCallback(TrackInfo)?.Invoke();
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
                var input = playable.GetInput(i);
                var inputPlayable = (ScriptPlayable<TweenBehaviour<TBinding>>)input;
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

        /// <summary>
        /// 開始前(プレビュー前)の状態を保存
        /// </summary>
        protected virtual void CacheOriginalState() { }
        
        /// <summary>
        /// 開始前(プレビュー前)の状態に戻す
        /// </summary>
        protected virtual void ResetToOriginalState() { }
    }
}