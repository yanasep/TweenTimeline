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
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 StartValue = new TimelineExpressionVector3Constant();

        [EnableIf(nameof(SetStartValue), true)]
        public TransformTweenPositionType positionType;

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(TweenTrackInfo<Transform> info)
        {
            if (!SetStartValue) return null;
            return () =>
            {
                switch (positionType)
                {
                    case TransformTweenPositionType.Position:
                        info.Target.position = StartValue.GetValue(info.Parameter);
                        break;
                    case TransformTweenPositionType.LocalPosition:
                        info.Target.localPosition = StartValue.GetValue(info.Parameter);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };   
        }
    }
}