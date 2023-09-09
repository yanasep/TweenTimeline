using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// 回転Tweenトラック
    /// </summary>
    [TrackClipType(typeof(TransformRotationTweenClip))]
    [TrackBindingType(typeof(RectTransform))]
    [DisplayName("Tween/Rotation Tween Track")]
    public class TransformRotationTweenTrack : TransformTweenTrack
    {   
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("RotateTool").image as Texture2D;  
#endif
        [SerializeField, ExtractContent] private RectTransformRotationTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<Transform> Template => _behaviour;
    }
    
    /// <summary>
    /// 回転Tweenミキサー
    /// </summary>
    [Serializable]
    public class RectTransformRotationTweenMixerBehaviour : TweenMixerBehaviour<Transform>
    {
        public bool SetStartValue;
        
        [EnableIf(nameof(SetStartValue), true)]
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 StartValue = new TimelineExpressionVector3Constant();

        private Quaternion _originalValue;

        /// <inheritdoc/>
        protected override void OnTrackStart()
        {
            if (!SetStartValue) return;
            Target.localRotation = Quaternion.Euler(StartValue.GetValue(Parameter));
        }

        /// <inheritdoc/>
        protected override void CacheOriginalState()
        {
            _originalValue = Target.localRotation;
        }

        /// <inheritdoc/>
        protected override void ResetToOriginalState()
        {
            Target.localRotation = _originalValue;
        }
    }
}