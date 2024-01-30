using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace TweenTimeline.Editor
{
    /// <summary>選択肢</summary>
    internal readonly struct SearchItem
    {
        public string Path { get; }
        public Texture2D Icon { get; }
        public object UserData { get; }
        public string Name => Path.Split('/').Last();

        /// <summary>コンストラクタ</summary>
        public SearchItem(string path, Texture2D icon = null, object userData = null)
        {
            Path = path;
            Icon = icon;
            UserData = userData;
        }
    }

    /// <summary>
    /// SearchButtonElementで表示するAdvancedDropdown
    /// </summary>
    internal class SearchButtonAdvancedDropdown : AdvancedDropdown
    {
        public event Action<SearchItem> OnItemSelected;

        private const int MinLineCount = 15;
        private const float MinWidth = 150;

        private readonly string title;
        private readonly IEnumerable<SearchItem> items;
        private readonly Dictionary<int, SearchItem> itemDic = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SearchButtonAdvancedDropdown(string title, IEnumerable<SearchItem> items, Action<SearchItem> onItemSelected, AdvancedDropdownState state) :
            base(state)
        {
            // 最小サイズを設定
            var minSize = minimumSize;
            minSize.x = MinWidth;
            minSize.y = MinLineCount * EditorGUIUtility.singleLineHeight;
            minimumSize = minSize;

            this.items = items;
            this.title = title;
            OnItemSelected = onItemSelected;
        }

        /// <inheritdoc/>
        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem(title);

            if (items == null) return root;

            foreach (var item in items)
            {
                if (item.Path == null) continue;
                var parts = item.Path.Split('/');
                var parent = root;
                for (var i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    if (i < parts.Length - 1)
                    {
                        var grandParent = parent;
                        parent = grandParent.children.FirstOrDefault(x => x.name == part);
                        if (parent == null)
                        {
                            parent = new AdvancedDropdownItem(part) { icon = item.Icon };
                            grandParent.AddChild(parent);
                        }

                        continue;
                    }

                    var dropdownItem = new AdvancedDropdownItem(part) { icon = item.Icon };
                    itemDic.Add(dropdownItem.id, item);
                    parent.AddChild(dropdownItem);
                }
            }

            return root;
        }

        /// <inheritdoc/>
        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            OnItemSelected?.Invoke(itemDic[item.id]);
        }
    }
}
