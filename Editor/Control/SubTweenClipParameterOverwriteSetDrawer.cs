// NOTE: Timelineパッケージのバージョンが1.8.1以上でないと、TimelineClipのインスペクターでUIToolkitが描画できない
#if UNITY_2023_1_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace TweenTimeline.Editor
{   
    [CustomPropertyDrawer(typeof(SubTweenClip.ParameterOverwriteSet))]
    public class SubTweenClipParameterOverwriteSetDrawer : PropertyDrawer
    {
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

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var set = property.GetValue<SubTweenClip.ParameterOverwriteSet>();
            
            var root = new VisualElement();
            TweenTimelineEditorResourceHolder.instance.SubTweenClipInspectorXml.CloneTree(root);
            root.Q<PropertyField>("TimelineAsset")
                .BindProperty(property.FindPropertyRelative(nameof(SubTweenClip.ParameterOverwriteSet.TimelineAsset)));
            
            _listView = root.Q<ListView>();
            _viewDataList ??= new();
            _parameterCandidates ??= new();
            GatherEntryViewData(set);
            GatherPropertyCandidates(property);
            
            _listView.makeItem = TweenTimelineEditorResourceHolder.instance.SubTweenClipOverwriteEntryXml.CloneTree;
            _listView.bindItem = (elem, i) =>
            {
                var data = _viewDataList[i];
                elem.Q<Label>("ParameterName").text = data.BindingData.ParameterName;
                elem.Q<Label>("ParameterType").text = data.ParameterType.ToString();
                var expressionProperty = property.FindPropertyRelative(data.BindingListName)
                    .GetArrayElementAtIndex(data.BindingListItemIndex).FindPropertyRelative("Expression");
                elem.Q<PropertyField>("Expression").BindProperty(expressionProperty);
            };
            _listView.itemsSource = _viewDataList;
            _listView.itemIndexChanged += (index1, index2) =>
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Reorder parameter overwrites");
                _viewDataList[index1].BindingData.ViewIndex = index1;
                _viewDataList[index2].BindingData.ViewIndex = index2;
            };
            
            var addButton = root.Q<Button>("add-button");
            addButton.clicked += () => AddItemAsync(VisualElementUtility.GetScreenPosition(addButton), property, set).Forget();
            var removeButton = root.Q<Button>("remove-button");
            removeButton.clicked += () => RemoveItem(property, set);

            root.TrackPropertyValue(property, OnPropertyValueChanged);
            return root;
        }

        private void OnPropertyValueChanged(SerializedProperty property)
        {
            var set = property.GetValue<SubTweenClip.ParameterOverwriteSet>();
            GatherEntryViewData(set);
            GatherPropertyCandidates(property);
            _listView.RefreshItems();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
        
        private async UniTask AddItemAsync(Vector2 position, SerializedProperty property, SubTweenClip.ParameterOverwriteSet set)
        {
            var options = _parameterCandidates.Select(x => $"{x.paramName} ({x.paramType})").ToArray();
            var selectedParam = await StringSearchWindow.OpenAsync("Parameter", options, new SearchWindowContext(position));
            Undo.RecordObject(property.serializedObject.targetObject, "Add parameter overwrite");
            var selectedIndex = Array.IndexOf(options, selectedParam);
            var (paramName, paramType) = _parameterCandidates[selectedIndex];
            var bindingList = SubTweenClipEditorUtility.GetParameterSetEntriesAsList(set, paramType);
            var bindingData = CreateBindingData(paramType);
            bindingData.ParameterName = paramName;
            bindingData.ViewIndex = _viewDataList.Count == 0 ? 0 : _viewDataList[^1].BindingData.ViewIndex + 1;
            bindingList.Add(bindingData);
            var viewData = new EntryViewData
            {
                BindingListName = GetParameterListName(paramType),
                ParameterType = paramType,
                BindingListItemIndex = bindingList.Count - 1,
                BindingData = bindingData
            };
            _viewDataList.Add(viewData);
            
            // focus added item
            _listView.selectedIndex = _viewDataList.Count - 1;
        }
        
        private void RemoveItem(SerializedProperty property, SubTweenClip.ParameterOverwriteSet set)
        {   
            Undo.RecordObject(property.serializedObject.targetObject, "Remove parameter overwrite");
            
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
        
            return;
            
            void remove(EntryViewData item)
            {
                _viewDataList.Remove(item);
                var dataList = SubTweenClipEditorUtility.GetParameterSetEntriesAsList(set, item.ParameterType);
                dataList.RemoveAt(item.BindingListItemIndex);   
            }
        }
        
        private string GetParameterListName(TweenParameterType type)
        {
            var typeName = type.ToString();
            return $"{char.ToUpper(typeName[0])}{typeName.Substring(1)}s";
        }
        
        private SubTweenClip.ParameterOverwrite CreateBindingData(TweenParameterType paramType)
        {
            var typeArg = TweenParameterEditorUtility.ParameterTypeToType(paramType);
            Type expressionType = GetExpressionType(typeArg);
            var constructedType = typeof(SubTweenClip.ParameterOverwrite<,>).MakeGenericType(expressionType, typeArg);
            var instance = Activator.CreateInstance(constructedType);
            return (SubTweenClip.ParameterOverwrite)instance;
        }

        private Type GetExpressionType(Type typeArg)
        {
            Type genericBooType = typeof(TweenTimelineExpression<>);

            var assembly = typeof(TweenTimelineExpression<>).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                if (type.BaseType != null && type.BaseType.IsGenericType &&
                    type.BaseType.GetGenericTypeDefinition() == genericBooType)
                {
                    var genericArguments = type.BaseType.GetGenericArguments();
                    if (genericArguments.Contains(typeArg))
                    {
                        return type;
                    }
                }
            }

            return null;
        }
        
        private void GatherEntryViewData(SubTweenClip.ParameterOverwriteSet set)
        {
            _viewDataList.Clear();
            AddEntries(set.Floats, nameof(set.Floats), TweenParameterType.Float);
            AddEntries(set.Ints, nameof(set.Ints), TweenParameterType.Int);
            AddEntries(set.Bools, nameof(set.Bools), TweenParameterType.Bool);
            AddEntries(set.Vector3s, nameof(set.Vector3s), TweenParameterType.Vector3);
            AddEntries(set.Vector2s, nameof(set.Vector2s), TweenParameterType.Vector2);
            AddEntries(set.Colors, nameof(set.Colors), TweenParameterType.Color);
            _viewDataList.Sort((a, b) => a.BindingData.ViewIndex.CompareTo(b.BindingData.ViewIndex));
            return;
        
            void AddEntries(IList entries, string listName, TweenParameterType parameterType)
            {
                if (entries == null) return;
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    var paramEntry = (SubTweenClip.ParameterOverwrite)entry;
                    _viewDataList.Add(new EntryViewData
                    {
                        BindingListName = listName,
                        BindingListItemIndex = i,
                        BindingData = paramEntry,
                        ParameterType = parameterType
                    });
                }
            }
        }

        private void GatherPropertyCandidates(SerializedProperty property)
        {
            _parameterCandidates.Clear();

            var timelineAsset = property.FindPropertyRelative(nameof(SubTweenClip.ParameterOverwriteSet.TimelineAsset)).GetValue<TimelineAsset>();
            if (timelineAsset == null) return;

            var paramTrack = TweenTimelineUtility.FindTweenParameterTrack(timelineAsset);
            if (paramTrack == null) return;

            var enumerable = Enumerable.Empty<(string paramName, TweenParameterType paramType, int index)>();
            AddCandidates(paramTrack.floats);
            AddCandidates(paramTrack.ints);
            AddCandidates(paramTrack.bools);
            AddCandidates(paramTrack.vector3s);
            AddCandidates(paramTrack.vector2s);
            AddCandidates(paramTrack.colors);
            _parameterCandidates.AddRange(enumerable.OrderBy(x => x.index).Select(x => (x.paramName, x.paramType)));
            return;

            void AddCandidates<T>(IEnumerable<TweenParameterTrack.ParameterSetEntry<T>> list)
            {
                foreach (var item in list)
                {
                    enumerable = enumerable.Append((item.Name, TweenParameterEditorUtility.TypeToParameterType(typeof(T)), item.ViewIndex));
                }
            }
        }
    }
}
#endif
