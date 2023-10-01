using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Yanasep;

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

        [SerializeField, ExtractContent] private ActivationTweenMixerBehaviour template;
        protected override TweenMixerBehaviour<GameObject> Template => template;

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);

#if UNITY_EDITOR
            var binding = director.GetGenericBinding(this) as Graphic;
            if (binding == null) return;
            driver.AddFromName(binding, "m_IsActive");
#endif
        }
    }

    [Serializable]
    public class ActivationTweenMixerBehaviour : TweenMixerBehaviour<GameObject>
    {
        protected override void OnStart(Playable playable)
        {
            Target.SetActive(false);
        }

        protected override void OnUpdate(Playable playable, double trackTime)
        {
            Target.SetActive(IsAnyClipPlaying(playable));
        }
    } 
}
