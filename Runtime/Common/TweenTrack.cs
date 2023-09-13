using System.ComponentModel;
using System.Reflection;
using System.Text;
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

        public abstract Tween CreateTween(Object binding);
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
            
            var playable = ScriptPlayable<TweenMixerBehaviour>.Create(graph, inputCount);
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

            var sb = new StringBuilder();
            sb.AppendLine("Sequence()");
            string Indent = "    ";

            // if (target is Transform trans)
            // {
            //     sequence.Append(trans.DOMove(new Vector3(150, 150), 3f));
            //     return sequence;
            // }
            
            sequence.AppendCallback(() => Debug.Log("sequence start!"));

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
                    Parameter = null
                };
                // start
                sequence.AppendCallback(() => Debug.Log("clip start!"));
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

                sequence.AppendCallback(() => Debug.Log("clip end!"));
            }

            var endCallback = GetEndCallback(tweenTrackInfo);
            if (endCallback != null)
            {
                sequence.AppendCallback(endCallback);
                sb.Append(Indent);
                sb.AppendLine(GetEndLog(tweenTrackInfo));
            }

            sequence.AppendCallback(() => Debug.Log("sequence end!"));
            sb.Append(";");

            Debug.Log($"CreateTween ({name}):\n{sb}");
            return sequence;
        }
    }
    
    /// <summary>
    /// TweenTimelineのMixerBehaviourのベースクラス
    /// </summary>
    public class TweenMixerBehaviour : PlayableBehaviour 
    {
        public Tween Tween { get; set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var duration = playable.GetGraph().GetRootPlayable(0).GetDuration();
            var trackTime = playable.GetTime() % duration;
            Tween.Rewind();
            Tween.Goto((float)trackTime);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            Tween.Rewind();
        }
    }
}