using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TweenTimeline.Editor
{
    /// <summary>
    /// 指定したstringリストからの検索window
    /// </summary>
    public class StringSearchWindow : SearchWindowBase<(string label, IList<string> options)>
    {
        /// <summary>
        /// 開く
        /// </summary>
        public static async UniTask<(string value, int index)> OpenAsync(string label, IList<string> options, SearchWindowContext context)
        {
            var result = (UserData)await OpenAsync<StringSearchWindow>((label, options), context);
            return (result.Value, result.Index);
        }
        
        private class UserData
        {
            public string Value;
            public int Index;
        }
        
        /// <summary>
        /// 検索結果のTree表示取得
        /// </summary>
        public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent(_arg.label)) { level = 0 });

            for (var i = 0; i < _arg.options.Count; i++)
            {
                string val = _arg.options[i];
                var userData = new UserData { Value = val, Index = i };
                entries.Add(new SearchTreeEntry(new GUIContent(val, _icon)) { level = 1, userData = userData });
            }

            return entries;
        }
    }
}
