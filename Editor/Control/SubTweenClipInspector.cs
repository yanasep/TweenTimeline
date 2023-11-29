using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TweenTimeline.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TweenTimeline
{
    [CustomEditor(typeof(SubTweenClip))]
    public class SubTweenClipInspector : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset _inspectorXml;
        [SerializeField] private VisualTreeAsset _entryXml;
        private SubTweenClip _clip;
        private ListView _listView;
        private List<EntryViewData> _viewDataList;
        private List<(string paramName, TweenParameterType paramType)> _parameterCandidates;

        private class EntryViewData
        {
            public string BindingListName;
            public TweenParameterType ParameterType;
            public int BindingListItemIndex;
            public SubTweenClip.ParameterOverwrite BindingData;
        }

        private void OnEnable()
        {
            _clip = (SubTweenClip)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            _inspectorXml.CloneTree(root);
            _listView = root.Q<ListView>();

            root.Bind(serializedObject);

            _viewDataList ??= new();
            GatherEntryViewData();
            _viewDataList.Sort((a, b) => a.BindingData.ViewIndex.CompareTo(b.BindingData.ViewIndex));

            _listView.makeItem = _entryXml.CloneTree;
            // _listView.bindItem = (elem, i) =>
            // {
            //     var data = _viewDataList[i];
            //     (elem as IBindable)!.bindingPath = $"{data.BindingListName}.Array.data[{data.BindingListItemIndex}]";
            //     elem.Bind(serializedObject);
            //     var typeEnumField = elem.Q<EnumField>();
            //     typeEnumField.SetValueWithoutNotify(data.ParameterType);
            //     typeEnumField.RegisterCallback<ChangeEvent<Enum>, EntryViewData>(OnParamTypeChange, data);
            // };
            // _listView.unbindItem = (elem, _) =>
            // {
            //     (elem as IBindable)!.bindingPath = "";
            // };
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
            // var options = _parameterCandidates.Select(x => $"{x.paramName} ({x.paramType})").ToArray();
            // var selectedParam = await SearchWindow.OpenAsync<StringSearchWindow, string, (string, string)>(position);
            // var selectedIndex = Array.IndexOf(options, selectedParam);
            // var (paramName, paramType) = _parameterCandidates[selectedIndex];
            // var bindingList = SubTweenClipEditorUtility.GetParameterSetEntriesAsList(_clip, paramType);
            // var bindingData = CreateBindingData(paramType);
            // bindingData.Name = "New Parameter";
            // bindingData.ViewIndex = _viewDataList.Count == 0 ? 0 : _viewDataList[^1].BindingData.ViewIndex + 1;
            // bindingList.Add(bindingData);
            // OnDataChanged();
            // var viewData = new EntryViewData
            // {
            //     BindingListName = GetParameterListName(paramName),
            //     ParameterType = paramName,
            //     BindingListItemIndex = bindingList.Count - 1,
            //     BindingData = bindingData
            // };
            // _viewDataList.Add(viewData);
            // _listView.RefreshItems();
            
            // focus added item
            _listView.selectedIndex = _viewDataList.Count - 1;
            // var elem = _listView.Q(className: "unity-collection-view__item--selected");
            // var text = elem.Q<TextField>();
            // text.Focus();
        }

        private void RemoveItem()
        {
            void remove(EntryViewData item)
            {
                _viewDataList.Remove(item);
                var dataList = SubTweenClipEditorUtility.GetParameterSetEntriesAsList(_clip, item.ParameterType);
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

        private TweenParameterTrack.ParameterSetEntry CreateBindingData(TweenParameterType paramType)
        {
            var typeArg = TweenParameterEditorUtility.ParameterTypeToType(paramType);
            // TODO
            Type expressionType = null;
            var constructedType = typeof(SubTweenClip.ParameterOverwrite<,>).MakeGenericType(expressionType, typeArg);
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
            AddEntries(_clip.floats, nameof(_clip.floats), TweenParameterType.Float);
            AddEntries(_clip.ints, nameof(_clip.ints), TweenParameterType.Int);
            AddEntries(_clip.bools, nameof(_clip.bools), TweenParameterType.Bool);
            AddEntries(_clip.vector3s, nameof(_clip.vector3s), TweenParameterType.Vector3);
            AddEntries(_clip.vector2s, nameof(_clip.vector2s), TweenParameterType.Vector2);
            AddEntries(_clip.colors, nameof(_clip.colors), TweenParameterType.Color);
            return;

            void AddEntries(IList entries, string listName, TweenParameterType parameterType)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    _viewDataList.Add(new EntryViewData
                    {
                        BindingListName = listName,
                        BindingListItemIndex = i,
                        BindingData = entry as SubTweenClip.ParameterOverwrite,
                        ParameterType = parameterType
                    });
                }
            }
        }
    }
}