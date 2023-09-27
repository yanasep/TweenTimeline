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
        public TweenTimelineField<Vector3> StartValue = new();

        [EnableIf(nameof(SetStartValue), true)]
        public TweenTimelineField<RectTransformTweenPositionType> PositionType = new();

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(TweenTrackInfo<RectTransform> info)
        {
            if (!SetStartValue) return null;

            return () =>
            {
                switch (PositionType.Value)
                {
                    case RectTransformTweenPositionType.AnchoredPosition:
                        info.Target.anchoredPosition = StartValue.Value;
                        break;
                    case RectTransformTweenPositionType.Position:
                        info.Target.position = StartValue.Value;
                        break;
                    case RectTransformTweenPositionType.LocalPosition:
                        info.Target.localPosition = StartValue.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        /// <inheritdoc/>
        protected override string GetStartLog(TweenTrackInfo<RectTransform> info)
        {
            return PositionType.Value switch
            {
                RectTransformTweenPositionType.AnchoredPosition => $"Set AnchoredPosition to {StartValue}",
                RectTransformTweenPositionType.Position => $"Set Position to {StartValue}",
                RectTransformTweenPositionType.LocalPosition => $"Set LocalPosition to {StartValue}",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
