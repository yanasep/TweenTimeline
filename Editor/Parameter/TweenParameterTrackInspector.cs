using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TweenTimeline.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using SearchWindow = TweenTimeline.Editor.SearchWindow;

namespace TweenTimeline
{
    [CustomEditor(typeof(TweenParameterTrack))]
    public class TweenParameterTrackInspector : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset _inspectorXml;
        [SerializeField] private VisualTreeAsset _entryXml;
        private TweenParameterTrack _track;
        private ListView _listView;
        private List<EntryViewData> _viewDataList;

        private class EntryViewData
        {
            public string BindingListName;
            public TweenParameterType ParameterType;
            public int BindingListItemIndex;
            public TweenParameterTrack.ParameterSetEntry BindingData;
        }

        private void OnEnable()
        {
            _track = (TweenParameterTrack)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            _inspectorXml.CloneTree(root);
            _listView = root.Q<ListView>();

            _viewDataList ??= new();
            GatherEntryViewData();
            _viewDataList.Sort((a, b) => a.BindingData.ViewIndex.CompareTo(b.BindingData.ViewIndex));

            _listView.makeItem = _entryXml.CloneTree;
            _listView.bindItem = (elem, i) =>
            {
                var data = _viewDataList[i];
                (elem as IBindable)!.bindingPath = $"{data.BindingListName}.Array.data[{data.BindingListItemIndex}]";
                elem.Bind(serializedObject);
                var typeEnumField = elem.Q<EnumField>();
                typeEnumField.SetValueWithoutNotify(data.ParameterType);
                typeEnumField.RegisterCallback<ChangeEvent<Enum>, EntryViewData>(OnParamTypeChange, data);
            };
            _listView.unbindItem = (elem, _) =>
            {
                (elem as IBindable)!.bindingPath = "";
                var typeEnumField = elem.Q<EnumField>();
                typeEnumField.UnregisterCallback<ChangeEvent<Enum>, EntryViewData>(OnParamTypeChange);
            };
            _listView.itemsSource = _viewDataList;
            _listView.itemIndexChanged += (index1, index2) =>
            {
                _viewDataList[index1].BindingData.ViewIndex = index1;
                _viewDataList[index2].BindingData.ViewIndex = index2;
            };

            var addButton = root.Q<Button>("add-button");
            addButton.clicked += () => AddItemAsync(VisualElementUtility.GetScreenPosition(addButton)).Forget();
            var removeButton = root.Q<Button>("remove-button");
            removeButton.clicked += () => RemoveItem();

            return root;
        }
        
        private async UniTask AddItemAsync(Vector2 position)
        {
            var type = await SearchWindow.OpenAsync<ParameterTypeSearchWindow, TweenParameterType>(new SearchWindowContext(position));
            var bindingList = TweenParameterEditorUtility.GetParameterSetEntriesAsList(_track, type);
            var bindingData = CreateBindingData(type);
            bindingData.Name = "New Parameter";
            bindingData.ViewIndex = _viewDataList.Count == 0 ? 0 : _viewDataList[^1].BindingData.ViewIndex + 1;
            bindingList.Add(bindingData);
            OnDataChanged();
            var viewData = new EntryViewData
            {
                BindingListName = GetParameterListName(type),
                ParameterType = type,
                BindingListItemIndex = bindingList.Count - 1,
                BindingData = bindingData
            };
            _viewDataList.Add(viewData);
            _listView.RefreshItems();
            
            // focus added item
            _listView.selectedIndex = _viewDataList.Count - 1;
            var elem = _listView.Q(className: "unity-collection-view__item--selected");
            var text = elem.Q<TextField>();
            text.Focus();
        }

        private void RemoveItem()
        {
            void remove(EntryViewData item)
            {
                _viewDataList.Remove(item);
                var dataList = TweenParameterEditorUtility.GetParameterSetEntriesAsList(_track, item.ParameterType);
                dataList.RemoveAt(item.BindingListItemIndex);   
            }
            
            // 何も選択していなければ最後の要素を削除
            if (_listView.selectedIndex == -1 && _viewDataList.Count > 0)
            {
                remove(_viewDataList[^1]);
            }
            else
            {
                var views = _listView.selectedIndices.Select(i => _viewDataList[i]).ToList();
                foreach (var view in views)
                {
                    remove(view);
                }
            }

            OnDataChanged();
            _listView.RefreshItems();
        }

        private string GetParameterListName(TweenParameterType type)
        {
            var typeName = type.ToString();
            return $"{char.ToLower(typeName[0])}{typeName.Substring(1)}s";
        }

        private void OnParamTypeChange(ChangeEvent<Enum> evt, EntryViewData data)
        {
            var newType = (TweenParameterType)evt.newValue;
            TweenParameterEditorUtility.GetParameterSetEntriesAsList(_track, data.ParameterType).RemoveAt(data.BindingListItemIndex);

            data.ParameterType = newType;
            var bindingData = CreateBindingData(newType);
            bindingData.Name = data.BindingData.Name;
            ParameterSetEntryConverter.TryConvert(prevData: data.BindingData, newData: bindingData);
            data.BindingData = bindingData;
            var bindingList = TweenParameterEditorUtility.GetParameterSetEntriesAsList(_track, newType);
            bindingList.Add(data.BindingData);
            data.BindingListItemIndex = bindingList.Count - 1;
            data.BindingListName = GetParameterListName(newType);
            OnDataChanged();
            _listView.RefreshItems();
        }

        private TweenParameterTrack.ParameterSetEntry CreateBindingData(TweenParameterType paramType)
        {
            var typeArg = TweenParameterEditorUtility.ParameterTypeToType(paramType);
            var constructedType = typeof(TweenParameterTrack.ParameterSetEntry<>).MakeGenericType(typeArg);
            var instance = Activator.CreateInstance(constructedType);
            return (TweenParameterTrack.ParameterSetEntry)instance;
        }

        private void OnDataChanged()
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            EditorUtility.SetDirty(target);
        }

        private void GatherEntryViewData()
        {
            _viewDataList.Clear();
            AddEntries(_track.floats, nameof(_track.floats), TweenParameterType.Float);
            AddEntries(_track.ints, nameof(_track.ints), TweenParameterType.Int);
            AddEntries(_track.bools, nameof(_track.bools), TweenParameterType.Bool);
            AddEntries(_track.vector3s, nameof(_track.vector3s), TweenParameterType.Vector3);
            AddEntries(_track.vector2s, nameof(_track.vector2s), TweenParameterType.Vector2);
            AddEntries(_track.colors, nameof(_track.colors), TweenParameterType.Color);
            return;

            void AddEntries<T>(List<TweenParameterTrack.ParameterSetEntry<T>> entries, string listName, TweenParameterType parameterType)
            {
                if (entries == null) return;
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    _viewDataList.Add(new EntryViewData
                    {
                        BindingListName = listName,
                        BindingListItemIndex = i,
                        BindingData = entry,
                        ParameterType = parameterType
                    });
                }
            }
        }
    }
}