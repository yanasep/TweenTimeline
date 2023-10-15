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
    /// 移動Tweenトラック
    /// </summary>
    [DisplayName("Tween/RectTransform Position Tween Track")]
    [TrackBindingType(typeof(RectTransform))]
    [TrackClipType(typeof(RectTransformPositionTweenClip))]
    [TrackClipType(typeof(RectTransformPositionKeepClip))]
    [TrackClipType(typeof(TransformPositionTweenClip))]
    public class RectTransformPositionTweenTrack : RectTransformTweenTrack<RectTransformPositionTweenMixerBehaviour>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("MoveTool").image as Texture2D;
#endif  
    }
    
    [Serializable]
    public class RectTransformPositionTweenMixerBehaviour : TweenMixerBehaviour<RectTransform>
    {
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        public TweenTimelineFieldVector3 StartValue;

        [EnableIf(nameof(SetStartValue), true)]
        public TweenTimelineField<RectTransformTweenPositionType> PositionType;
        
        protected override void OnStart(Playable playable)
        {
            base.OnStart(playable);
            if (!SetStartValue) return;

            Target.SetPosition(PositionType.Value, StartValue.Value);
        }
    }
}
