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
        
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector2 StartValue = new TimelineExpressionVector2Constant { Value = new Vector2(100, 100) };

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(TweenTrackInfo<RectTransform> info)
        {
            if (!SetStartValue) return null;
            return () => { info.Target.sizeDelta = StartValue.GetValue(info.Parameter); };
        }
    }
}