using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// サブTweenトラック
    /// </summary>
    [DisplayName("Tween/Sub Tween Track")]
    [TrackBindingType(typeof(TweenTimelineDirector))]
    [TrackClipType(typeof(SubTweenClip))]
    public class SubTweenTrack : TweenTrack<TweenTimelineDirector>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("d_UnityEditor.Timeline.TimelineWindow").image as Texture2D;
#endif

        public TweenTimelineField<ActivationType> activationType;

        public enum ActivationType
        {
            DoNothing, ActiveWhilePlaying, ActiveOnStart
        }

        protected override TweenCallback GetStartCallback(TweenTrackInfo<TweenTimelineDirector> info)
        {
            if (activationType.Value is ActivationType.ActiveOnStart or ActivationType.ActiveWhilePlaying)
            {
                return () => info.Target.gameObject.SetActive(true);
            }

            return null;
        }

        protected override TweenCallback GetEndCallback(TweenTrackInfo<TweenTimelineDirector> info)
        {
            if (activationType.Value is ActivationType.ActiveWhilePlaying)
            {
                return () => info.Target.gameObject.SetActive(false);
            }

            return null;
        }

        public override Tween CreateTween(CreateTweenArgs args)
        {
            var tween = base.CreateTween(args);

            if (activationType.Value == ActivationType.DoNothing)
            {
                return tween;
            }

            var go = ((TweenTimelineDirector)args.Binding).gameObject;

            var sequence = DOTween.Sequence();
            sequence.Append(tween);

            var inputs = GetClipInputs();

            sequence.Join(DOTweenEx.EveryUpdate((float)timelineAsset.duration, t =>
            {
                if (go == null) return;

                bool active = activationType.Value == ActivationType.ActiveOnStart && HasAnyClipStarted(inputs, t)
                    || activationType.Value == ActivationType.ActiveWhilePlaying && IsAnyClipPlaying(inputs, t);

                go.SetActive(active);
            }));

            return sequence;
        }
    }
}
