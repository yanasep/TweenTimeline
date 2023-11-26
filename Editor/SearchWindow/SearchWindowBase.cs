using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TweenTimeline.Editor
{
    public static class SearchWindow
    {
        public static UniTask<TResult> OpenAsync<TSearchWindow, TResult>(Vector2 position, 
            float requestedWidth = 0.0f,
            float requestedHeight = 0.0f) 
            where TSearchWindow : SearchWindowProviderBase<TResult>
        {
            var _completionSource = new UniTaskCompletionSource<TResult>();
            var searchWindowProvider = ScriptableObject.CreateInstance<TSearchWindow>();
            searchWindowProvider.Initialize(onSelectEntry: val =>
            {
                _completionSource.TrySetResult(val);
                Object.DestroyImmediate(searchWindowProvider);
            });
            searchWindowProvider.onDestroy += () => _completionSource.TrySetCanceled();
            UnityEditor.Experimental.GraphView.SearchWindow.Open(new SearchWindowContext(position, requestedWidth, requestedHeight), searchWindowProvider);
            return _completionSource.Task;
        }
    }

    public abstract class SearchWindowProviderBase<T> : SearchWindowProviderBase
    {
        public void Initialize(Action<T> onSelectEntry)
        {
            base.Initialize(obj => onSelectEntry?.Invoke(CastResult(obj)));
        }

        protected virtual T CastResult(object result)
        {
            return (T)result;
        }
    }
    
    /// <summary>
    /// 検索windowのベースクラス
    /// </summary>
    public abstract class SearchWindowProviderBase : ScriptableObject, ISearchWindowProvider
    {
        private Action<object> _onSelectEntry;
        protected Texture2D _icon;
        public event Action onDestroy;

        protected void Initialize(Action<object> onSelectEntry)
        {
            _onSelectEntry = onSelectEntry;
            _icon = new Texture2D(1, 1);
            _icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _icon.Apply();
        }

        /// <summary>
        /// 検索結果のTree表示取得
        /// </summary>
        public abstract List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context);

        /// <summary>
        /// OnSelectEntry
        /// </summary>
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            _onSelectEntry?.Invoke(searchTreeEntry.userData);
            return true;
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            if (_icon != null)
            {
                DestroyImmediate(_icon);
                _icon = null;
            }

            onDestroy?.Invoke();
        }
    }
}
