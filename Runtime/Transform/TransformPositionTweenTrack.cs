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
    [DisplayName("Tween/Position Tween Track")]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(TransformPositionTweenClip))]
    // [TrackClipType(typeof(TransformPositionKeepClip))]
    public class TransformPositionTweenTrack : TransformTweenTrack
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("MoveTool").image as Texture2D;  
#endif
        
        public bool SetStartValue;

        [EnableIf(nameof(SetStartValue), true)]
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 StartValue = new TweenTimelineExpressionVector3Constant { Value = Vector3.zero };

        [EnableIf(nameof(SetStartValue), true)]
        public TransformTweenPositionType PositionType;

        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(CreateTweenArgs args)
        {
            if (!SetStartValue) return null;

            var target = (Transform)args.Binding;
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
        }
    }
}