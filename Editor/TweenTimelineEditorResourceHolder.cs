using UnityEditor;
using UnityEngine.UIElements;

namespace TweenTimeline.Editor
{
    /// <summary>
    /// エディタ用のリソース参照保持
    /// </summary>
    public class TweenTimelineEditorResourceHolder : ScriptableSingleton<TweenTimelineEditorResourceHolder>
    {
        // スクリプトのDefault Referenceでセット
        public VisualTreeAsset SubTweenClipInspectorXml;
        public VisualTreeAsset SubTweenClipOverwriteEntryXml;
    }
}
