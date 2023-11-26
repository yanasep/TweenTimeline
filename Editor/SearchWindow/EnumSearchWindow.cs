using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TweenTimeline.Editor
{
    /// <summary>
    /// Typeの検索window
    /// </summary>
    public class EnumSearchWindowProvider<T> : SearchWindowProviderBase<T> where T : Enum
    {
        protected override T CastResult(object result)
        {
            var val = (int)result;
            return (T)Enum.ToObject(typeof(T), val);
        }

        /// <summary>
        /// 検索結果のTree表示取得
        /// </summary>
        public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent(typeof(T).Name)) { level = 0 });

            foreach (var val in Enum.GetValues(typeof(T)))
            {
                var enumName = Enum.GetName(typeof(T), val);
                entries.Add(new SearchTreeEntry(new GUIContent(enumName, _icon)) { level = 1, userData = val });
            }
            return entries;
        }
    }
}
