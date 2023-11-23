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
    /// アクティベーションTweenトラック
    /// </summary>
    [DisplayName("Tween/Activation Track")]
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(ActivationTweenClip))]
    [TrackColor(0.2f, 1f, 0.2f)]
    public class ActivationTweenTrack : TweenTrack<GameObject>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("d_Toggle Icon").image as Texture2D;
#endif
        
        public ActivationControlPlayable.PostPlaybackState postPlayback = ActivationControlPlayable.PostPlaybackState.Revert;

        /// <inheritdoc/>
        public override Tween CreateTween(CreateTweenArgs args)
        {
            var go = args.Binding as GameObject;
            if (go == null) return null;
            return CreateTween(this, go, args.Duration, postPlayback);
        }

        public static Tween CreateTween(TweenTrack track, GameObject target, float duration, ActivationControlPlayable.PostPlaybackState postPlayback)
        {
            var inputs = track.GetClipInputs();
            var initialActive = target.activeSelf;
            var tween = DOTweenEx.EveryUpdate(duration, t =>
            {
                target.SetActive(inputs.IsAnyPlaying(t));
            });

            switch (postPlayback)
            {
                case ActivationControlPlayable.PostPlaybackState.Active:
                    tween.OnKill(() => target.SetActive(true));
                    break;
                case ActivationControlPlayable.PostPlaybackState.Inactive:
                    tween.OnKill(() => target.SetActive(false));
                    break;
                case ActivationControlPlayable.PostPlaybackState.Revert:
                    tween.OnKill(() => target.SetActive(initialActive));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return tween;
        }

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);

#if UNITY_EDITOR
            var binding = director.GetGenericBinding(this) as GameObject;
            if (binding == null) return;
            driver.AddFromName(binding, "m_IsActive");
#endif
        }
    }
}
