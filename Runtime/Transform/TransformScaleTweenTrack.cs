using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// スケールTweenトラック
    /// </summary>
    [TrackClipType(typeof(TransformScaleTweenClip))]
    [TrackBindingType(typeof(RectTransform))]
    [DisplayName("Tween/Scale Tween Track")]
    public class TransformScaleTweenTrack : TransformTweenTrack
    {   
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("ScaleTool").image as Texture2D;  
#endif
        [SerializeField, ExtractContent] private TransformScaleTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<Transform> Template => _behaviour;
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
        protected override void OnTrackStart()
        {
            if (!SetStartValue) return;
            Target.localScale = StartValue.GetValue(Parameter);
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