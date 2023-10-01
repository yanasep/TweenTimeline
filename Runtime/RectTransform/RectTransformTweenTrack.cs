using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// RectTransformTweenトラックのベース
    /// </summary>
    [TrackColor(0.851f, 0.843f, 0.945f)]
    public abstract class RectTransformTweenTrack<TMixerBehaviour> : TweenTrack<RectTransform, TMixerBehaviour>
        where TMixerBehaviour : TweenMixerBehaviour<RectTransform>
    {   
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            var comp = director.GetGenericBinding(this) as RectTransform;
            if (comp == null)
                return;
            var so = new UnityEditor.SerializedObject(comp);
            var iter = so.GetIterator();
            while (iter.NextVisible(true))
            {
                if (iter.hasVisibleChildren)
                    continue;
                driver.AddFromName<RectTransform>(comp.gameObject, iter.propertyPath);
            }
#endif
            base.GatherProperties(director, driver);
        }
    }
}
