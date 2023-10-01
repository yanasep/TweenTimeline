using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// CanvasGroupTweenトラックのベース
    /// </summary>
    [TrackColor(0.2f, 0.2f, 0.2f)]
    public abstract class TextMeshProUGUITweenTrack<TTweenMixerBehaviour> : TweenTrack<TextMeshProUGUI, TTweenMixerBehaviour>
        where TTweenMixerBehaviour : TweenMixerBehaviour<TextMeshProUGUI>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("TextMesh Icon").image as Texture2D;
#endif

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            var comp = director.GetGenericBinding(this) as TextMeshProUGUI;
            if (comp == null)
                return;
            var so = new UnityEditor.SerializedObject(comp);
            var iter = so.GetIterator();
            while (iter.NextVisible(true))
            {
                if (iter.hasVisibleChildren)
                    continue;
                driver.AddFromName<TextMeshProUGUI>(comp.gameObject, iter.propertyPath);
            }
#endif
            base.GatherProperties(director, driver);
        }
    }
}
