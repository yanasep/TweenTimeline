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
    /// サイズTweenトラック
    /// </summary>
    [DisplayName("Tween/RectTransform Size Tween Track")]
    [TrackBindingType(typeof(RectTransform))]
    [TrackClipType(typeof(RectTransformSizeDeltaTweenClip))]
    public class RectTransformSizeDeltaTweenTrack : RectTransformTweenTrack<RectTransformSizeDeltaTweenMixerBehaviour>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("RectTool").image as Texture2D;
#endif
    }

    [Serializable]
    public class RectTransformSizeDeltaTweenMixerBehaviour : TweenMixerBehaviour<RectTransform>
    {
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        public TweenTimelineField<Vector2> StartValue;

        protected override void OnStart(Playable playable)
        {
            base.OnStart(playable);
            if (!SetStartValue) return;
            Target.sizeDelta = StartValue.Value;
        }   
    }
}