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
    /// スケールTweenトラック
    /// </summary>
    [DisplayName("Tween/Scale Tween Track")]
    [TrackBindingType(typeof(RectTransform))]
    [TrackClipType(typeof(TransformScaleTweenClip))]
    [TrackClipType(typeof(TransformPunchScaleTweenClip))]
    public class TransformScaleTweenTrack : TransformTweenTrack<TransformScaleTweenMixerBehaviour>
    {   
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("ScaleTool").image as Texture2D;  
#endif
    }
    
    [Serializable]
    public class TransformScaleTweenMixerBehaviour : TweenMixerBehaviour<Transform>
    {
        [SerializeField] private bool setStartValue;

        [EnableIf(nameof(setStartValue), true)]
        [SerializeField]
        private TweenTimelineField<Vector3> startValue;

        protected override void OnStart(Playable playable)
        {
            base.OnStart(playable);
            if (!setStartValue) return;
            Target.localScale = startValue.Value;   
        }
    }
}