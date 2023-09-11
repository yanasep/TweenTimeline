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
        [SerializeField, ExtractContent] private TransformScaleTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<Transform> template => _behaviour;
    }
    
    /// <summary>
    /// スケールTweenミキサー
    /// </summary>
    [Serializable]
    public class TransformScaleTweenMixerBehaviour : TweenMixerBehaviour<Transform>
    {
        public bool SetStartValue;
        
        [EnableIf(nameof(SetStartValue), true)]
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 StartValue = new TimelineExpressionVector3Constant();

        private Vector3 _originalValue;

        /// <inheritdoc/>
        public override TweenCallback OnStartCallback 
        {
            get
            {   
                if (!SetStartValue) return null;
                return () =>
                {
                    Target.localScale = StartValue.GetValue(Parameter);
                };   
            }
        }

        /// <inheritdoc/>
        protected override void CacheOriginalState()
        {
            _originalValue = Target.localScale;
        }

        /// <inheritdoc/>
        protected override void ResetToOriginalState()
        {
            Target.localScale = _originalValue;
        }
    }
}