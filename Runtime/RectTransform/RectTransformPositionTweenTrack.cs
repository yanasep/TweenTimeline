using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// 移動Tweenトラック
    /// </summary>
    [TrackClipType(typeof(RectTransformPositionTweenClip))]
    [TrackClipType(typeof(TransformPositionTweenClip))]
    [TrackBindingType(typeof(RectTransform))]
    [DisplayName("Tween/Position Track")]
    public class RectTransformPositionTweenTrack : RectTransformTweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("MoveTool").image as Texture2D;  
#endif
        [SerializeField, ExtractContent] private RectTransformPositionTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<RectTransform> Template => _behaviour;
    }
    
    /// <summary>
    /// 移動Tweenミキサー
    /// </summary>
    [Serializable]
    public class RectTransformPositionTweenMixerBehaviour : TweenMixerBehaviour<RectTransform>
    {
        public bool SetStartValue;
        
        [EnableIf(nameof(SetStartValue), true)]
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector2 StartValue = new TimelineExpressionVector2Constant();

        [EnableIf(nameof(SetStartValue), true)]
        public RectTransformTweenPositionType positionType;

        private Vector3 _originalValue;

        /// <inheritdoc/>
        protected override void OnTrackStart()
        {
            if (!SetStartValue) return;
            
            switch (positionType)
            {
                case RectTransformTweenPositionType.AnchoredPosition:
                    Target.anchoredPosition = StartValue.GetValue(Parameter);
                    break;
                case RectTransformTweenPositionType.Position:
                    Target.position = StartValue.GetValue(Parameter);
                    break;
                case RectTransformTweenPositionType.LocalPosition:
                    Target.localPosition = StartValue.GetValue(Parameter);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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