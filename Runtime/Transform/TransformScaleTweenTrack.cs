using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// スケールTweenトラック
    /// </summary>
    [DisplayName("Tween/Scale Tween Track")]
    [TrackBindingType(typeof(RectTransform))]
    [TrackClipType(typeof(TransformScaleTweenClip))]
    [TrackClipType(typeof(TransformPunchScaleTweenClip))]
    public class TransformScaleTweenTrack : TransformTweenTrack
    {   
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("ScaleTool").image as Texture2D;  
#endif
        
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 StartValue = new TweenTimelineExpressionVector3Constant(Vector3.one);

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(CreateTweenArgs args)
        {
            if (!SetStartValue) return null;
            var target = (RectTransform)args.Binding;
            return () => target.localScale = StartValue.Evaluate(args.Parameter);
        }

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);

            var binding = director.GetGenericBinding(this) as Transform;
            if (binding == null) return;
            driver.AddFromName<Transform>(binding.gameObject, "m_LocalScale.x");
            driver.AddFromName<Transform>(binding.gameObject, "m_LocalScale.y");
            driver.AddFromName<Transform>(binding.gameObject, "m_LocalScale.z");
            driver.AddFromName<Transform>(binding.gameObject, "m_ConstrainProportionsScale");
        }
    }
}