using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Nestトラック
    /// </summary>
    [DisplayName("Tween/Nest Track")]
    [TrackBindingType(typeof(TimelineAsset))]
    [TrackClipType(typeof(NestClip))]
    public class NestTrack : TweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("d_scenevis_visible_hover").image as Texture2D;
#endif

        /// <inheritdoc/>
        public override Tween CreateTween(CreateTweenArgs args)
        {
            args.Binding
        }

        /// <inheritdoc/>
        public override string GetTweenString(CreateTweenArgs args)
        {
        }
    }
}
