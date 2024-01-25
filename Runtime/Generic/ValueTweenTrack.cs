using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace TweenTimeline
{
    /// <summary>
    /// Value Tweenトラック
    /// </summary>
    [DisplayName("Tween/Value Track")]
    [TrackClipType(typeof(IntTweenClip))]
    public class ValueTweenTrack : TweenTrackNoBinding
    {
// #if UNITY_EDITOR
//         public override Texture2D Icon => EditorGUIUtility.IconContent("Grid.FillTool").image as Texture2D;
// #endif
    }
}
