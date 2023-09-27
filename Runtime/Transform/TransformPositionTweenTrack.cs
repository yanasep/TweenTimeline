using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
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
    public class TransformPositionTweenTrack : TransformTweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("MoveTool").image as Texture2D;  
#endif
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        public TweenTimelineField<Vector3> StartValue;

        [EnableIf(nameof(SetStartValue), true)]
        public TweenTimelineField<TransformTweenPositionType> positionType;

        /// <inheritdoc/>
        protected override TweenCallback GetStartCallback(TweenTrackInfo<Transform> info)
        {
            if (!SetStartValue) return null;
            return () =>
            {
                switch (positionType.Value)
                {
                    case TransformTweenPositionType.Position:
                        info.Target.position = StartValue.Value;
                        break;
                    case TransformTweenPositionType.LocalPosition:
                        info.Target.localPosition = StartValue.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };   
        }
    }
}