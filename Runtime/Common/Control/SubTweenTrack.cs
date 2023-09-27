using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// サブTweenトラック
    /// </summary>
    [DisplayName("Tween/Sub Tween Track")]
    [TrackBindingType(typeof(TweenTimelineDirector))]
    [TrackClipType(typeof(SubTweenClip))]
    public class SubTweenTrack : TweenTrack<TweenTimelineDirector>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("d_UnityEditor.Timeline.TimelineWindow").image as Texture2D;
#endif
    }
}
