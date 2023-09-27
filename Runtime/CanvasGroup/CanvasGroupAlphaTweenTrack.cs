using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
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
    public class CanvasGroupAlphaTweenTrack : CanvasGroupTweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("d_scenevis_visible_hover").image as Texture2D;
#endif
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        public float StartValue = 1f;

        protected override TweenCallback GetStartCallback(TweenTrackInfo<CanvasGroup> info)
        {
            if (!SetStartValue) return null;
            return () => info.Target.alpha = StartValue;
        }
    }
}