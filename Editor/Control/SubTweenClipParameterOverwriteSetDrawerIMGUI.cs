// NOTE: Timelineパッケージのバージョンが1.8.1以上でないと、TimelineClipのインスペクターでUIToolkitが描画できない
#if !UNITY_2023_1_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

        private class ListItemData
        {
            public string BindingListName;
            public TweenParameterType ParameterType;
            public int BindingListItemIndex;
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
            foreach (var data in _viewDataList)
            {
                height += GetElementHeight(property, data) + EditorGUIUtility.standardVerticalSpacing;
            }

            height += _listView.headerHeight + _listView.footerHeight;
            
            // なんかちょっと足りないので適当に余分
            height += EditorGUIUtility.standardVerticalSpacing * 3;
            
            return height;
        }

        private float GetElementHeight(SerializedProperty property, ListItemData data)
        {
            var expressionProperty = property.FindPropertyRelative(data.BindingListName)
                .GetArrayElementAtIndex(data.BindingListItemIndex).FindPropertyRelative("Expression");
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
                var paramName = _parameterCandidates.FirstOrDefault(x => data.BindingData.ParameterId == x.paramId).paramName;
                var labelRect = rect;
                labelRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(labelRect, paramName);
                var expressionRect = rect;
                expressionRect.yMin = labelRect.yMax;
                var expressionProperty = property.FindPropertyRelative(data.BindingListName)
                    .GetArrayElementAtIndex(data.BindingListItemIndex).FindPropertyRelative("Expression");
                EditorGUI.PropertyField(expressionRect, expressionProperty);
            };
            _listView.onReorderCallbackWithDetails += (list, oldIndex, newIndex) =>
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Reorder parameter overwrites");
                _viewDataList[newIndex].BindingData.ViewIndex = newIndex;
            };
            _listView.onCanAddCallback += _ => true;
            _listView.onAddCallback += _ => AddItemAsync(property, set).Forget();
            _listView.onRemoveCallback += _ => RemoveItem(property, set);
        }

        private async UniTask AddItemAsync(SerializedProperty property, SubTweenClip.ParameterOverwriteSet set)
        {
            // 既に追加済みのパラメータは除外
            var options = _parameterCandidates
                .Where(x => !_viewDataList.Any(viewData =>
                    viewData.ParameterType == x.paramType && viewData.BindingData.ParameterId == x.paramId))
                .ToArray();
            var optionNames = options.Select(x => $"{x.paramName} ({x.paramType})").ToArray();
            var (_, selectedIndex) = await StringSearchWindow.OpenAsync("Parameter", optionNames, new SearchWindowContext(_listViewPosition));
            Undo.RecordObject(property.serializedObject.targetObject, "Add parameter overwrite");
            var (paramId, _, paramType) = options[selectedIndex];
            var bindingList = SubTweenClipEditorUtility.GetParameterSetEntriesAsList(set, paramType);
            var bindingData = CreateBindingData(paramType);
            bindingData.ParameterId = paramId;
            bindingData.ViewIndex = _viewDataList.Count == 0 ? 0 : _viewDataList[^1].BindingData.ViewIndex + 1;
            bindingList.Add(bindingData);
            var viewData = new ListItemData
            {
                BindingListName = GetParameterListName(paramType),
                ParameterType = paramType,
                BindingListItemIndex = bindingList.Count - 1,
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
            var instance = (SubTweenClip.ParameterOverwrite)Activator.CreateInstance(constructedType);
            var expressionField = constructedType.GetField("Expression", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            expressionField?.SetValue(instance, CreateConstantExpression(typeArg));
            return instance;
        }

        private static Type GetExpressionType(Type typeArg)
        {
            Type genericType = typeof(TweenTimelineExpression<>);

            var assembly = typeof(TweenTimelineExpression<>).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                if (type.BaseType != null && type.BaseType.IsGenericType &&
                    type.BaseType.GetGenericTypeDefinition() == genericType)
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

        private static object CreateConstantExpression(Type typeArg)
        {
            var baseType = typeof(TweenTimelineExpression<>).MakeGenericType(typeArg);
            var assembly = typeof(TweenTimelineExpression<>).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsSubclassOf(baseType)) continue;
                var displayName = type.GetCustomAttribute<DisplayNameAttribute>();
                if (displayName is { DisplayName: "Constant" })
                {
                    return Activator.CreateInstance(type);
                }
            }

            return null;
        }

        private void GatherListItemData(SubTweenClip.ParameterOverwriteSet set)
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
                        BindingListName = listName,
                        BindingListItemIndex = i,
                        BindingData = paramEntry,
                        ParameterType = parameterType,
                    });
                }
            }
        }

        private void GatherPropertyCandidates(SerializedProperty property)
        {
            _parameterCandidates.Clear();

            var timelineAsset = property.FindPropertyRelative(nameof(SubTweenClip.ParameterOverwriteSet.TimelineAsset)).GetValue<TimelineAsset>();
            if (timelineAsset == null) return;

            var parameterTrack = TweenTimelineUtility.FindTweenParameterTrack(timelineAsset);
            if (parameterTrack == null) return;

            var enumerable = Enumerable.Empty<(uint paramId, string paramName, TweenParameterType paramType, int index)>();
            AddCandidates(parameterTrack.floats);
            AddCandidates(parameterTrack.ints);
            AddCandidates(parameterTrack.bools);
            AddCandidates(parameterTrack.vector3s);
            AddCandidates(parameterTrack.vector2s);
            AddCandidates(parameterTrack.colors);
            _parameterCandidates.AddRange(enumerable.OrderBy(x => x.index).Select(x => (x.paramId, x.paramName, x.paramType)));
            return;

            void AddCandidates<T>(IEnumerable<TweenParameterTrack.ParameterSetEntry<T>> list)
            {
                foreach (var item in list)
                {
                    enumerable = enumerable.Append((item.ParameterId, item.ParameterName, TweenParameterEditorUtility.TypeToParameterType(typeof(T)), item.ViewIndex));
                }
            }
        }
    }
}
#endif
