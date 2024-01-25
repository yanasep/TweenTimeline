using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
        
        [SerializeField] private bool setStartValue;

        [EnableIf(nameof(setStartValue), true)]
        [SerializeReference, SelectableSerializeReference]
        [SerializeField] private TweenTimelineExpressionVector3 startValue = new TweenTimelineExpressionVector3Constant();

        /// <inheritdoc/>
        protected override TweenCallback GetStartCallback(CreateTweenArgs args)
        {
            if (!setStartValue) return null;
            var target = (RectTransform)args.Binding;
            return () => target.localRotation = Quaternion.Euler(startValue.Evaluate(args.Parameter));
        }

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);

            var binding = director.GetGenericBinding(this) as Transform;
            if (binding == null) return;
            driver.AddFromName<Transform>(binding.gameObject, "m_LocalRotation.x");
            driver.AddFromName<Transform>(binding.gameObject, "m_LocalRotation.y");
            driver.AddFromName<Transform>(binding.gameObject, "m_LocalRotation.z");
            driver.AddFromName<Transform>(binding.gameObject, "m_LocalRotation.w");
        }
    }
}