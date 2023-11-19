using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace TweenTimeline
{
    [DisplayName("Tween/Sub Tween Track")]
    [TrackBindingType(typeof(PlayableDirector))]
    [TrackClipType(typeof(SubTweenClip))]
    public class SubTweenTrack : TweenTrack<PlayableDirector>
    {
#if UNITY_EDITOR
        // public override Texture2D Icon => EditorGUIUtility.IconContent("Grid.FillTool").image as Texture2D;
#endif
    }
}