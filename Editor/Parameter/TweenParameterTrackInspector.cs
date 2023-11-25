using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TweenTimeline
{
    public enum TweenParameterType
    {
        Int = 0,
        Float = 1,
        Bool = 2,
        Vector3 = 3,
        Vector2 = 4,
        Color = 5,
    }

    [CustomEditor(typeof(TweenParameterTrack))]
    public class TweenParameterTrackInspector : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset inspectorXml;
        [SerializeField] private VisualTreeAsset entryXml;
        private TweenParameterTrack _track;
        private ListView _listView;
        private List<ListItem> paramList;

        private class ListItem
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
            inspectorXml.CloneTree(root);
            _listView = root.Q<ListView>();

            paramList ??= new();
            GatherParamListData();
            paramList.Sort((a, b) => a.BindingData.ViewIndex.CompareTo(b.BindingData.ViewIndex));

            _listView.makeItem = entryXml.CloneTree;
            _listView.bindItem = (elem, i) =>
            {
                var data = paramList[i];
                (elem as IBindable)!.bindingPath = $"{data.BindingListName}.Array.data[{data.BindingListItemIndex}]";
                elem.Bind(serializedObject);
                var typeEnumField = elem.Q<EnumField>();
                typeEnumField.SetValueWithoutNotify(data.ParameterType);
                typeEnumField.RegisterCallback<ChangeEvent<Enum>, ListItem>(OnParamTypeChange, data);
            };
            _listView.unbindItem = (elem, _) =>
            {
                (elem as IBindable)!.bindingPath = "";
                var typeEnumField = elem.Q<EnumField>();
                typeEnumField.UnregisterCallback<ChangeEvent<Enum>, ListItem>(OnParamTypeChange);
            };
            _listView.itemsSource = paramList;
            _listView.itemIndexChanged += (index1, index2) =>
            {
                paramList[index1].BindingData.ViewIndex = index1;
                paramList[index2].BindingData.ViewIndex = index2;
            };

            // var addButton = _listView.Q<Button>("unity-list-view__add-button");
            var addButton = root.Q<Button>("add-button");
            addButton.clicked += () =>
            {
                // TODO: Type選択ポップアップ
                var type = TweenParameterType.Int;
                var bindingList = GetDataList(type);
                var bindingData = CreateBindingData(type);
                bindingData.Name = "New Parameter";
                bindingData.ViewIndex = paramList[^1].BindingData.ViewIndex + 1;
                bindingList.Add(bindingData);
                var viewData = new ListItem
                {
                    BindingListName = ParameterTypeToListName(type),
                    ParameterType = type,
                    BindingListItemIndex = bindingList.Count - 1,
                    BindingData = bindingData
                };
                paramList.Add(viewData);
                _listView.RefreshItems();
            };
            var removeButton = root.Q<Button>("remove-button");
            removeButton.clicked += () =>
            {
                Debug.Log($"TODO");
            };

            return root;
        }

        private string ParameterTypeToListName(TweenParameterType type)
        {
            var name = type.ToString();
            return $"{name.Substring(0).ToLower()}{name.Substring(1)}s";
        }

        private void OnParamTypeChange(ChangeEvent<Enum> evt, ListItem data)
        {
            var prevType = (TweenParameterType)evt.previousValue;
            var newType = (TweenParameterType)evt.newValue;
        }

        private IList GetDataList(TweenParameterType type)
        {
            return type switch
            {
                TweenParameterType.Int => _track.ints,
                TweenParameterType.Float => _track.floats,
                TweenParameterType.Bool => _track.bools,
                TweenParameterType.Vector3 => _track.vector3s,
                TweenParameterType.Vector2 => _track.vector2s,
                TweenParameterType.Color => _track.colors,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private Type ParameterTypeToType(TweenParameterType type)
        {
            return type switch
            {
                TweenParameterType.Int => typeof(int),
                TweenParameterType.Float => typeof(float),
                TweenParameterType.Bool => typeof(bool),
                TweenParameterType.Vector3 => typeof(Vector3),
                TweenParameterType.Vector2 => typeof(Vector2),
                TweenParameterType.Color => typeof(Color),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };   
        }

        private TweenParameterTrack.ParameterSetEntry CreateBindingData(TweenParameterType paramType)
        {
            var typeArg = ParameterTypeToType(paramType);
            var constructedType = typeof(TweenParameterTrack.ParameterSetEntry<>).MakeGenericType(typeArg);
            var instance = Activator.CreateInstance(constructedType);
            return (TweenParameterTrack.ParameterSetEntry)instance;
        }

        private void GatherParamListData()
        {
            paramList.Clear();
            AddEntries(_track.floats, nameof(_track.floats), TweenParameterType.Float);
            AddEntries(_track.ints, nameof(_track.ints), TweenParameterType.Int);
            AddEntries(_track.bools, nameof(_track.bools), TweenParameterType.Bool);
            AddEntries(_track.vector3s, nameof(_track.vector3s), TweenParameterType.Vector3);
            AddEntries(_track.vector2s, nameof(_track.vector2s), TweenParameterType.Vector2);
            AddEntries(_track.colors, nameof(_track.colors), TweenParameterType.Color);
            return;

            void AddEntries<T>(List<TweenParameterTrack.ParameterSetEntry<T>> entries, string listName, TweenParameterType parameterType)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    paramList.Add(new ListItem
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