// NOTE: Timelineパッケージのバージョンが1.8.1以上でないと、TimelineClipのインスペクターでUIToolkitが描画できない
#if !UNITY_2023_1_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Timeline;

namespace TweenTimeline.Editor
{
    [CustomPropertyDrawer(typeof(SubTweenClip.ParameterOverwriteSet))]
    public class SubTweenClipParameterOverwriteSetDrawerIMGUI : PropertyDrawer
    {
        private ReorderableList _listView;
        private readonly List<ListItemData> _viewDataList = new();
        private readonly List<(uint paramId, string paramName, TweenParameterType paramType)> _parameterCandidates = new();
        private bool initialized;
        private Vector3 _listViewPosition;
        private TweenParameterTrack _paramTrack;
        private GUIContent _parameterMissingContent;
        private GUIContent _typeMismatchContent;

        private class ListItemData
        {
            public SubTweenClip.ParameterOverwrite BindingData;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);

            var timelineProp = property.FindPropertyRelative(nameof(SubTweenClip.ParameterOverwriteSet.TimelineAsset));
            var timelinePos = position;
            timelinePos.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(timelinePos, timelineProp);

            var listPos = position;
            listPos.yMin = timelinePos.yMax + EditorGUIUtility.standardVerticalSpacing;
            _listView.DoList(listPos);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            float height = 0;

            // timeline asset
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // list
            height += _listView.GetHeight();

            return height;
        }

        private float GetElementHeight(SerializedProperty property, ListItemData data)
        {
            var set = property.GetValue<SubTweenClip.ParameterOverwriteSet>();
            var (listPath, listIndex) = set.GetPropertyPath(data.BindingData.ParameterId);
            var expressionProperty = property.FindPropertyRelative(listPath)
                .GetArrayElementAtIndex(listIndex).FindPropertyRelative("Expression");
            var expressionHeight = EditorGUI.GetPropertyHeight(expressionProperty);
            return EditorGUIUtility.singleLineHeight + expressionHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void Initialize(SerializedProperty property)
        {
            if (initialized) return;
            initialized = true;

            var set = property.GetValue<SubTweenClip.ParameterOverwriteSet>();

            GatherListItemData(set);
            GatherPropertyCandidates(property);

            _parameterMissingContent = new GUIContent(EditorGUIUtility.IconContent("console.warnicon.sml"));
            _parameterMissingContent.text = "Target Parameter is missing";
            _typeMismatchContent = new GUIContent(EditorGUIUtility.IconContent("console.warnicon.sml"));
            _typeMismatchContent.text = "Type mismatch";

            _listView = new ReorderableList(_viewDataList, typeof(ListItemData));
            _listView.elementHeightCallback += i =>
            {
                var data = _viewDataList[i];
                return GetElementHeight(property, data);
            };
            _listView.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Parameter Overwrites");
            _listView.drawElementCallback = (rect, index, active, focused) =>
            {
                var data = _viewDataList[index];
                var (listPath, listIndex) = set.GetPropertyPath(data.BindingData.ParameterId);
                var entry = _paramTrack.GetEntry(data.BindingData.ParameterId);
                var targetExists = entry != null;
                var labelRect = rect;
                labelRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(labelRect, targetExists ? new GUIContent(entry.ParameterName) : _parameterMissingContent);
                var expressionRect = rect;
                expressionRect.yMin = labelRect.yMax;
                if (data.BindingData.TargetParameterType != _paramTrack.GetParameterType(data.BindingData.ParameterId))
                {
                    EditorGUI.LabelField(expressionRect, _typeMismatchContent);
                }
                else
                {
                    var expressionProperty = property.FindPropertyRelative(listPath)
                        .GetArrayElementAtIndex(listIndex).FindPropertyRelative("Expression");
                    EditorGUI.PropertyField(expressionRect, expressionProperty);
                }
            };
            _listView.onReorderCallbackWithDetails += (list, oldIndex, newIndex) =>
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Reorder parameter overwrites");
                _viewDataList[newIndex].BindingData.ViewIndex = newIndex;
            };
            _listView.onAddCallback += _ => AddItemAsync(property, set).Forget();
            _listView.onRemoveCallback += _ => RemoveItem(property, set);
        }

        private async UniTask AddItemAsync(SerializedProperty property, SubTweenClip.ParameterOverwriteSet set)
        {
            // 既に追加済みのパラメータは除外
            var options = _parameterCandidates
                .Where(x => _viewDataList.All(viewData => viewData.BindingData.ParameterId != x.paramId))
                .ToArray();
            var optionNames = options.Select(x => $"{x.paramName} ({x.paramType})").ToArray();
            var (_, selectedIndex) = await StringSearchWindow.OpenAsync("Parameter", optionNames, new SearchWindowContext(_listViewPosition));
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
            _listView.Select(_viewDataList.Count - 1);
        }

        private void RemoveItem(SerializedProperty property, SubTweenClip.ParameterOverwriteSet set)
        {
            Undo.RecordObject(property.serializedObject.targetObject, "Remove parameter overwrite");

            // 何も選択していなければ最後の要素を削除
            if (_listView.selectedIndices.Count == 0 && _viewDataList.Count > 0)
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

        private void GatherListItemData(SubTweenClip.ParameterOverwriteSet set)
        {
            _viewDataList.Clear();
            AddEntries(set.Floats);
            AddEntries(set.Ints);
            AddEntries(set.Bools);
            AddEntries(set.Vector3s);
            AddEntries(set.Vector2s);
            AddEntries(set.Colors);
            _viewDataList.Sort((a, b) => a.BindingData.ViewIndex.CompareTo(b.BindingData.ViewIndex));
            return;

            void AddEntries(IList entries)
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
