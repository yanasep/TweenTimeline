using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
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
    public class TransformScaleTweenTrack : TransformTweenTrack
    {   
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("ScaleTool").image as Texture2D;  
#endif
     
        [SerializeField] private bool setStartValue;
        
        [EnableIf(nameof(setStartValue), true)]
        [SerializeReference, SelectableSerializeReference] 
        private TimelineExpressionVector3 startValue = new TimelineExpressionVector3Constant();

        protected override TweenCallback GetStartCallback(TweenTrackInfo<Transform> info)
        {
            if (!setStartValue) return null;
            return () =>
            {
                info.Target.localScale = startValue.GetValue(info.Parameter);
            };   
        }
    }
}