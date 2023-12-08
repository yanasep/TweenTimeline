using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TweenTimeline.Editor
{
    /// <summary>
    /// 指定したstringリストからの検索window
    /// </summary>
    public class StringSearchWindow : SearchWindowProviderBase<string, (string label, IList<string> options)>
    {
        public static UniTask<string> OpenAsync(string label, IList<string> options, SearchWindowContext context)
        {
            return SearchWindow.OpenAsync<StringSearchWindow, string, (string label, IList<string> options)>((label, options), context);
        }
        
        /// <summary>
        /// 検索結果のTree表示取得
        /// </summary>
        public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent(_arg.label)) { level = 0 });

            foreach (var val in _arg.options)
            {
                entries.Add(new SearchTreeEntry(new GUIContent(val, _icon)) { level = 1, userData = val });
            }
            return entries;
        }
    }
}
