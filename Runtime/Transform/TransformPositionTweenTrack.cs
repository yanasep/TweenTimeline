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
        [SerializeField, ExtractContent] private TransformPositionTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<Transform> template => _behaviour;
    }
    
    /// <summary>
    /// 移動Tweenミキサー
    /// </summary>
    [Serializable]
    public class TransformPositionTweenMixerBehaviour : TweenMixerBehaviour<Transform>
    {
        public bool SetStartValue;
        
        [EnableIf(nameof(SetStartValue), true)]
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 StartValue = new TimelineExpressionVector3Constant();

        [EnableIf(nameof(SetStartValue), true)]
        public TransformTweenPositionType positionType;

        private Vector3 _originalValue;

        /// <inheritdoc/>
        public override TweenCallback OnStartCallback 
        {
            get
            {   
                if (!SetStartValue) return null;
                return () =>
                {
                    switch (positionType)
                    {
                        case TransformTweenPositionType.Position:
                            Target.position = StartValue.GetValue(Parameter);
                            break;
                        case TransformTweenPositionType.LocalPosition:
                            Target.localPosition = StartValue.GetValue(Parameter);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                };   
            }
        }

        /// <inheritdoc/>
        protected override void CacheOriginalState()
        {
            _originalValue = Target.position;
        }

        /// <inheritdoc/>
        protected override void ResetToOriginalState()
        {
            Target.position = _originalValue;
        }
    }
}