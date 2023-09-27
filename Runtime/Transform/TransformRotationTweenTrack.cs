using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// 回転Tweenトラック
    /// </summary>
    [DisplayName("Tween/Rotation Tween Track")]
    [TrackBindingType(typeof(RectTransform))]
    [TrackClipType(typeof(TransformRotationTweenClip))]
    public class TransformRotationTweenTrack : TransformTweenTrack
    {   
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("RotateTool").image as Texture2D;  
#endif
     
        [SerializeField] private bool setStartValue;

        [EnableIf(nameof(setStartValue), true)]
        [SerializeField] private TweenTimelineField<Vector3> startValue;

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(TweenTrackInfo<Transform> info)
        {
            if (!setStartValue) return null;
            return () =>
            {
                info.Target.localRotation = Quaternion.Euler(startValue.Value);
            };   
        }
    }
}