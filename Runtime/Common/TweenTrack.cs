using System;
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
    /// TweenTimelineのTrackのベースクラス
    /// </summary>
    public abstract class TweenTrack<TBinding> : TweenTrack where TBinding : Object
    {
        // Unityの不具合？でTrackの最初のfoldoutが表示されないっぽいので適当なフィールドで回避
        [SerializeField, ReadOnly] private byte _;

        public virtual TweenCallback GetStartCallback(CreateTweenArgs args) => null;
        
        public override Tween CreateTween(CreateTweenArgs args)
        {
            var target = args.Binding as TBinding;
            if (target == null) return null;
            
            var sequence = DOTween.Sequence().Pause().SetAutoKill(false);

            var startCallback = GetStartCallback(args);
            if (startCallback != null)
            {
                sequence.AppendCallback(startCallback);
            }

            double currentTime = 0;
            foreach (var clip in GetClips())
            {
                // interval
                var tweenClip = (TweenClip<TBinding>)clip.asset;
                float interval = (float)(clip.start - currentTime);
                currentTime = clip.start + clip.duration;
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
            var tween = CreateTween(new CreateTweenArgs
            {
                Binding = binding,
                Parameter = parameter,
                Duration = (float)timelineAsset.duration
            });
            if (tween == null) return base.CreateTrackMixer(graph, go, inputCount);
            var playable = ScriptPlayable<TweenMixerBehaviour>.Create(graph, inputCount);
            playable.GetBehaviour().Tween = tween;
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
    }
    
    /// <summary>
    /// TweenTimelineのMixerBehaviour
    /// </summary>
    public class TweenMixerBehaviour : PlayableBehaviour
    {
        public Tween Tween { get; set; }
        private PlayableDirector director;

        /// <inheritdoc/>
        public override void OnPlayableCreate(Playable playable)
        {
            director = playable.GetGraph().GetResolver() as PlayableDirector;
        }

        /// <inheritdoc/>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {   
            var trackTime = (float)GetTrackTime(playable.GetTime(), playable.GetGraph().GetRootPlayable(0).GetDuration());
            Tween.GotoWithCallbacks(trackTime);
        }

        /// <inheritdoc/>
        public override void OnPlayableDestroy(Playable playable)
        {
            Tween?.Kill();
        }

        private double GetTrackTime(double time, double duration)
        {
            return director.extrapolationMode switch
            {
                DirectorWrapMode.Loop => time % duration,
                _ => Math.Min(time, duration)
            };
        }
    }
}