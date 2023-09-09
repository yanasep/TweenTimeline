using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// CanvasGroupTweenトラックのベース
    /// </summary>
    [TrackColor(0.2f, 0.2f, 0.2f)]
    public abstract class TextMeshProUGUITweenTrack : TweenTrack<TextMeshProUGUI>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("TextMesh Icon").image as Texture2D;
#endif
    }
}
