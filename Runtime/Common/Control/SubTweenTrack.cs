using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// サブTweenトラック
    /// </summary>
    [DisplayName("Tween/Sub Tween Track")]
    [TrackBindingType(typeof(PlayableDirector))]
    [TrackClipType(typeof(SubTweenClip))]
    public class SubTweenTrack : TweenTrack<PlayableDirector>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("d_UnityEditor.Timeline.TimelineWindow").image as Texture2D;
#endif

        public TweenTimelineField<ActivationType> activationType;

        public enum ActivationType
        {
            DoNothing, ActiveWhilePlaying, ActiveOnStart
        }

        protected override TweenCallback GetStartCallback(TweenTrackInfo<PlayableDirector> info)
        {
            if (activationType.Value is ActivationType.ActiveOnStart or ActivationType.ActiveWhilePlaying)
            {
                return () => info.Target.gameObject.SetActive(true);
            }

            return null;
        }

        protected override TweenCallback GetEndCallback(TweenTrackInfo<PlayableDirector> info)
        {
            if (activationType.Value is ActivationType.ActiveWhilePlaying)
            {
                return () => info.Target.gameObject.SetActive(false);
            }

            return null;
        }

        protected override Action<float> GetUpdateCallback(TweenTrackInfo<PlayableDirector> info)
        {
            if (activationType.Value == ActivationType.DoNothing)
            {
                return null;
            }

            var inputs = GetClipInputs();

            return t =>
            {
                var go = info.Target.gameObject;

                bool active = activationType.Value == ActivationType.ActiveOnStart && HasAnyClipStarted(inputs, t)
                              || activationType.Value == ActivationType.ActiveWhilePlaying && IsAnyClipPlaying(inputs, t);

                go.SetActive(active);
            };
        }
        
        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);

#if UNITY_EDITOR
            var comp = director.GetGenericBinding(this) as PlayableDirector;
            if (comp == null) return;
            var go = comp.gameObject;
            driver.AddFromName(go, "m_IsActive");
#endif
        }
    }
}
