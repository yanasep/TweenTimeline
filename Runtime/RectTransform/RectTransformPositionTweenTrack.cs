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
    [DisplayName("Tween/RectTransform Position Tween Track")]
    [TrackBindingType(typeof(RectTransform))]
    [TrackClipType(typeof(RectTransformPositionTweenClip))]
    [TrackClipType(typeof(RectTransformPositionKeepClip))]
    [TrackClipType(typeof(TransformPositionTweenClip))]
    public class RectTransformPositionTweenTrack : RectTransformTweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("MoveTool").image as Texture2D;  
#endif

        public bool SetStartValue;
        
        [EnableIf(nameof(SetStartValue), true)]
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 StartValue = new TimelineExpressionVector3Constant();

        [EnableIf(nameof(SetStartValue), true)]
        public RectTransformTweenPositionType PositionType;

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(TweenTrackInfo<RectTransform> info)
        {
            if (!SetStartValue) return null;
            
            return () =>
            {
                switch (PositionType)
                {
                    case RectTransformTweenPositionType.AnchoredPosition:
                        info.Target.anchoredPosition = StartValue.GetValue(info.Parameter);
                        break;
                    case RectTransformTweenPositionType.Position:
                        info.Target.position = StartValue.GetValue(info.Parameter);
                        break;
                    case RectTransformTweenPositionType.LocalPosition:
                        info.Target.localPosition = StartValue.GetValue(info.Parameter);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };  
        }
    }
}