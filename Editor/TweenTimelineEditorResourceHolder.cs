using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TweenTimeline.Editor
{
    public class TweenTimelineEditorResourceHolder : ScriptableSingleton<TweenTimelineEditorResourceHolder>
    {
        public VisualTreeAsset SubTweenClipInspectorXml;
        public VisualTreeAsset SubTweenClipOverwriteEntryXml;
    }
}
