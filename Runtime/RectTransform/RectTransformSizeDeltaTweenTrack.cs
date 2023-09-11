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
    /// サイズTweenトラック
    /// </summary>
    [DisplayName("Tween/RectTransform Size Tween Track")]
    [TrackBindingType(typeof(RectTransform))]
    [TrackClipType(typeof(RectTransformSizeDeltaTweenClip))]
    public class RectTransformSizeDeltaTweenTrack : RectTransformTweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("RectTool").image as Texture2D;
#endif
        [SerializeField, ExtractContent] private RectTransformSizeDeltaTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<RectTransform> template => _behaviour;
    }

    /// <summary>
    /// サイズTweenミキサー
    /// </summary>
    [Serializable]
    public class RectTransformSizeDeltaTweenMixerBehaviour : TweenMixerBehaviour<RectTransform>
    {
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector2 StartValue = new TimelineExpressionVector2Constant { Value = new Vector2(100, 100) };

        private Vector2 _originalValue;

        /// <inheritdoc/>
        public override TweenCallback OnStartCallback
        {
            get
            {
                if (!SetStartValue) return null;
                return () => { Target.sizeDelta = StartValue.GetValue(Parameter); };
            }
        }

        /// <inheritdoc/>
        protected override void CacheOriginalState()
        {
            _originalValue = Target.sizeDelta;
        }

        /// <inheritdoc/>
        protected override void ResetToOriginalState()
        {
            Target.sizeDelta = _originalValue;
        }
    }
}