using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// 移動Tweenトラック
    /// </summary>
    [DisplayName("Tween/RectTransform Position Tween Track")]
    [TrackBindingType(typeof(RectTransform))]
    [TrackClipType(typeof(RectTransformPositionTweenClip))]
    // [TrackClipType(typeof(RectTransformPositionKeepClip))]
    [TrackClipType(typeof(TransformPositionTweenClip))]
    public class RectTransformPositionTweenTrack : RectTransformTweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("MoveTool").image as Texture2D;
#endif
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 StartValue = new TweenTimelineExpressionVector3Constant { Value = Vector3.zero };

        [EnableIf(nameof(SetStartValue), true)]
        public RectTransformTweenPositionType PositionType;

        /// <inheritdoc/>
        protected override TweenCallback GetStartCallback(CreateTweenArgs args)
        {
            if (!SetStartValue) return null;

            var target = (RectTransform)args.Binding;
            var startPos = StartValue.Evaluate(args.Parameter);
            return () =>
            {
                target.SetPosition(PositionType, startPos);
            };
        }

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);

            var binding = director.GetGenericBinding(this) as Transform;
            if (binding == null) return;
            driver.AddFromName<Transform>(binding.gameObject, "m_LocalPosition.x");
            driver.AddFromName<Transform>(binding.gameObject, "m_LocalPosition.y");
            driver.AddFromName<Transform>(binding.gameObject, "m_LocalPosition.z");
            driver.AddFromName<Transform>(binding.gameObject, "m_AnchoredPosition.x");
            driver.AddFromName<Transform>(binding.gameObject, "m_AnchoredPosition.y");
        }
    }
}
