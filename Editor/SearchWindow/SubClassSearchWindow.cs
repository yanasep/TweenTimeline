using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TweenTimeline.Editor
{
    /// <summary>
    /// Typeの検索window
    /// </summary>
    public class SubClassSearchWindow<T> : SearchWindowBase<string>
    {
        /// <summary>
        /// 開く
        /// </summary>
        public static UniTask<Type> OpenAsync(SearchWindowContext context)
        {
            var label = typeof(T).Name;
            return OpenAsync(label, context);
        }
        
        /// <summary>
        /// 開く
        /// </summary>
        public static async UniTask<Type> OpenAsync(string label, SearchWindowContext context)
        {
            var result = await OpenAsync<SubClassSearchWindow<T>>(label, context);
            return (Type)result;
        }

        /// <summary>
        /// 検索結果のTree表示取得
        /// </summary>
        public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent(typeof(T).Name)) { level = 0 });
            
            foreach (var type in GetAllTypes())
            {
                if (!type.IsAbstract && (type.IsSubclassOf(typeof(T)) || typeof(T).IsAssignableFrom(type)))
                {
                    entries.Add(new SearchTreeEntry(new GUIContent(type.Name, _icon)) { level = 1, userData = type });
                }
            }
            return entries;
        }

        private IEnumerable<Type> GetAllTypes()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    yield return type;
                }
            }
        }
    }
}
