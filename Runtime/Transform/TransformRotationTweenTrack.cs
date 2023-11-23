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
        [SerializeReference, SelectableSerializeReference]
        [SerializeField] private TweenTimelineExpressionVector3 startValue = new TweenTimelineExpressionVector3Constant();

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(CreateTweenArgs args)
        {
            if (!setStartValue) return null;
            var target = (RectTransform)args.Binding;
            return () => target.localRotation = Quaternion.Euler(startValue.Evaluate(args.Parameter));
        }
    }
}