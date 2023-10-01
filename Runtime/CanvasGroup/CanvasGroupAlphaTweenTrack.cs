using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// CanvasGroupのAlphaのTweenトラック
    /// </summary>
    [DisplayName("Tween/Canvas Group Alpha Tween Track")]
    [TrackBindingType(typeof(CanvasGroup))]
    [TrackClipType(typeof(CanvasGroupAlphaTweenClip))]
    public class CanvasGroupAlphaTweenTrack : CanvasGroupTweenTrack<CanvasGroupAlphaTweenMixerBehaviour>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("d_scenevis_visible_hover").image as Texture2D;
#endif
    }

    [Serializable]
    public class CanvasGroupAlphaTweenMixerBehaviour : TweenMixerBehaviour<CanvasGroup>
    {
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        public float StartValue = 1f;

        /// <inheritdoc/>
        protected override void OnStart(Playable playable)
        {
            if (!SetStartValue) return;
            Target.alpha = StartValue;
        }  
    }
}