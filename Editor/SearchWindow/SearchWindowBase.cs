using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TweenTimeline.Editor
{
    /// <summary>
    /// 検索windowのベースクラス
    /// </summary>
    public abstract class SearchWindowBase<TResult> : SearchWindowBase<TResult, object>
    {
        /// <summary>
        /// 開く
        /// </summary>
        protected static UniTask<TResult> OpenAsync<TSearchWindow>(SearchWindowContext context)
            where TSearchWindow : SearchWindowBase<TResult>
        {
            return OpenAsync<TSearchWindow>(null, context);
        }
    }

    /// <summary>
    /// 検索windowのベースクラス
    /// </summary>
    public abstract class SearchWindowBase<TResult, TArg> : SearchWindowBase
    {
        /// <summary>
        /// 開く
        /// </summary>
        protected static UniTask<TResult> OpenAsync<TSearchWindow>(TArg arg, SearchWindowContext context)
            where TSearchWindow : SearchWindowBase<TResult, TArg>
        {
            var _completionSource = new UniTaskCompletionSource<TResult>();
            var searchWindowProvider = CreateInstance<TSearchWindow>();
            searchWindowProvider.Initialize(onSelectEntry: val =>
            {
                _completionSource.TrySetResult(val);
                DestroyImmediate(searchWindowProvider);
            }, arg);
            searchWindowProvider.onDestroy += () => _completionSource.TrySetCanceled();
            SearchWindow.Open(context, searchWindowProvider);
            return _completionSource.Task;
        }

        protected TArg _arg;
        
        /// <summary>
        /// 初期化
        /// </summary>
        private void Initialize(Action<TResult> onSelectEntry, TArg arg)
        {
            base.Initialize(obj => onSelectEntry?.Invoke(CastResult(obj)));

            _arg = arg;
        }

        /// <summary>
        /// 選択されたSearchTreeEntryのuserDataをリザルトに変換
        /// </summary>
        protected virtual TResult CastResult(object result)
        {
            return (TResult)result;
        }
    }
    
    /// <summary>
    /// 検索windowのベースクラス
    /// </summary>
    public abstract class SearchWindowBase : ScriptableObject, ISearchWindowProvider
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
