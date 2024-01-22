using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
        public TweenTimelineExpressionVector2 StartValue = new TweenTimelineExpressionVector2Constant { Value = Vector3.zero };

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(CreateTweenArgs args)
        {
            if (!SetStartValue) return null;

            var target = (RectTransform)args.Binding;
            return () =>
            {
                target.sizeDelta = StartValue.Evaluate(args.Parameter);
            };
        }

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);

            var binding = director.GetGenericBinding(this) as RectTransform;
            if (binding == null) return;
            driver.AddFromName<RectTransform>(binding.gameObject, "m_SizeDelta.x");
            driver.AddFromName<RectTransform>(binding.gameObject, "m_SizeDelta.y");
        }
    }
}