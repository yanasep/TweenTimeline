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
        [SerializeField, ExtractContent] private RectTransformRotationTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<Transform> template => _behaviour;
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
        public override TweenCallback OnStartCallback 
        {
            get
            {   
                if (!SetStartValue) return null;
                return () =>
                {
                    Target.localRotation = Quaternion.Euler(StartValue.GetValue(Parameter));
                };   
            }
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