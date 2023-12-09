using System;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

namespace TweenTimeline.Editor
{
    /// <summary>
    /// Enumの検索window
    /// </summary>
    public static class EnumSearchWindow
    {
        /// <summary>
        /// 開く
        /// </summary>
        public static UniTask<T> OpenAsync<T>(SearchWindowContext context) where T : struct, Enum
        {
            return OpenAsync<T>(typeof(T).Name, context);
        }
        
        /// <summary>
        /// 開く
        /// </summary>
        public static async UniTask<T> OpenAsync<T>(string label, SearchWindowContext context) where T : struct, Enum
        {
            var options = Enum.GetNames(typeof(T));
            var result = await StringSearchWindow.OpenAsync(label, options, context);
            return Enum.Parse<T>(result);
        }
    }
}
