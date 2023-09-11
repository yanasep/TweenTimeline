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
    /// CanvasGroupのAlphaのTweenトラック
    /// </summary>
    [DisplayName("Tween/Canvas Group Alpha Tween Track")]
    [TrackBindingType(typeof(CanvasGroup))]
    [TrackClipType(typeof(CanvasGroupAlphaTweenClip))]
    public class CanvasGroupAlphaTweenTrack : CanvasGroupTweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("d_scenevis_visible_hover").image as Texture2D;
#endif
        [SerializeField, ExtractContent] private CanvasGroupAlphaTweenMixerBehaviour _behaviour;
        protected override TweenMixerBehaviour<CanvasGroup> template => _behaviour;
    }
    
    /// <summary>
    /// CanvasGroupのAlphaのTweenミキサー
    /// </summary>
    [Serializable]
    public class CanvasGroupAlphaTweenMixerBehaviour : TweenMixerBehaviour<CanvasGroup>
    {
        public bool SetStartValue;
        
        [EnableIf(nameof(SetStartValue), true)]
        public float StartValue = 1f;

        private float _originalValue;

        /// <inheritdoc/>
        public override TweenCallback OnStartCallback 
        {
            get
            {   
                if (!SetStartValue) return null;
                return () =>
                {
                    Target.alpha = StartValue;
                };   
            }
        }

        /// <inheritdoc/>
        protected override void CacheOriginalState()
        {
            _originalValue = Target.alpha;
        }

        /// <inheritdoc/>
        protected override void ResetToOriginalState()
        {
            Target.alpha = _originalValue;
        }
    }
}