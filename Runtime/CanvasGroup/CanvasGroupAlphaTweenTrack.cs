using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
        
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionFloat StartValue = new TweenTimelineExpressionFloatConstant();

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(CreateTweenArgs args)
        {
            if (!SetStartValue) return null;

            var target = (CanvasGroup)args.Binding;
            return () =>
            {
                target.alpha = StartValue.Evaluate(args.Parameter);
            };
        }

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);

            var binding = director.GetGenericBinding(this) as CanvasGroup;
            if (binding == null) return;
            driver.AddFromName<CanvasGroup>(binding.gameObject, "m_Alpha");
        }
    }
}