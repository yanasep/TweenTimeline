// NOTE: Timelineパッケージのバージョンが1.8.1以上でないと、TimelineClipのインスペクターでUIToolkitが描画できない
#if UNITY_2023_1_OR_NEWER
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
        private List<ListItemData> _viewDataList;
        private List<(uint paramId, string paramName, TweenParameterType paramType)> _parameterCandidates;
        private TweenParameterTrack _paramTrack;

        private class ListItemData
        {
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
            
            _listView.makeItem = TweenTimelineEditorResourceHolder.instance.SubTweenClipOverwriteEntryXml.Instantiate;
            _listView.bindItem = (elem, i) =>
            {
                var data = _viewDataList[i];

                var (listPath, listIndex) = set.GetPropertyPath(data.BindingData.ParameterId);
                elem.Q<Label>("ParameterName").text = _paramTrack.GetEntry(data.BindingData.ParameterId).ParameterName;
                var expressionProperty = property.FindPropertyRelative(listPath)
                    .GetArrayElementAtIndex(listIndex).FindPropertyRelative("Expression");
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
            var options = _parameterCandidates
                .Where(x => _viewDataList.All(viewData => viewData.BindingData.ParameterId != x.paramId))
                .ToArray();
            var optionNames = options.Select(x => $"{x.paramName} ({x.paramType})").ToArray();
            var (_, selectedIndex) = await StringSearchWindow.OpenAsync("Parameter", optionNames, new SearchWindowContext(position));
            Undo.RecordObject(property.serializedObject.targetObject, "Add parameter overwrite");
            var (paramId, _, paramType) = options[selectedIndex];
            var bindingData = set.AddEntry(paramId, TweenParameterEditorUtility.ParameterTypeToType(paramType));
            bindingData.ViewIndex = _viewDataList.Count == 0 ? 0 : _viewDataList[^1].BindingData.ViewIndex + 1;
            var viewData = new ListItemData
            {
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
            
            void remove(ListItemData item)
            {
                _viewDataList.Remove(item);
                set.RemoveEntry(item.BindingData.ParameterId);  
            }
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
                    _viewDataList.Add(new ListItemData
                    {
                        BindingData = paramEntry,
                    });
                }
            }
        }

        private void GatherPropertyCandidates(SerializedProperty property)
        {
            _parameterCandidates.Clear();

            var timelineAsset = property.FindPropertyRelative(nameof(SubTweenClip.ParameterOverwriteSet.TimelineAsset)).GetValue<TimelineAsset>();
            if (timelineAsset == null) return;

            _paramTrack = TweenTimelineUtility.FindTweenParameterTrack(timelineAsset);
            if (_paramTrack == null) return;

            var enumerable = Enumerable.Empty<(uint paramId, string paramName, TweenParameterType paramType, int index)>();
            AddCandidates(_paramTrack.floats);
            AddCandidates(_paramTrack.ints);
            AddCandidates(_paramTrack.bools);
            AddCandidates(_paramTrack.vector3s);
            AddCandidates(_paramTrack.vector2s);
            AddCandidates(_paramTrack.colors);
            _parameterCandidates.AddRange(enumerable.OrderBy(x => x.index).Select(x => (x.paramId, x.paramName, x.paramType)));
            return;

            void AddCandidates<T>(IEnumerable<TweenParameterTrack.ParameterSetEntry<T>> list)
            {
                foreach (var item in list)
                {
                    enumerable = enumerable.Append((item.ParameterId, item.ParameterName,
                        TweenParameterEditorUtility.TypeToParameterType(typeof(T)), item.ViewIndex));
                }
            }
        }
    }
}
#endif
