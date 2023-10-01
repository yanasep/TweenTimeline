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
    [DisplayName("Tween/Position Tween Track")]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(TransformPositionTweenClip))]
    [TrackClipType(typeof(TransformPositionKeepClip))]
    public class TransformPositionTweenTrack : TransformTweenTrack<TransformPositionTweenMixerBehaviour>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("MoveTool").image as Texture2D;  
#endif
    }
    
    [Serializable]
    public class TransformPositionTweenMixerBehaviour : TweenMixerBehaviour<Transform>
    {
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        public TweenTimelineField<Vector3> StartValue;

        [EnableIf(nameof(SetStartValue), true)]
        public TweenTimelineField<TransformTweenPositionType> positionType;

        protected override void OnStart(Playable playable)
        {
            if (!SetStartValue) return;

            Target.SetPosition(positionType.Value, StartValue.Value);
        }   
    }
}