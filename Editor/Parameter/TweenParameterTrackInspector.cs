using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TweenTimeline.Editor
{
    /// <summary>
    /// TweenParameterTrackのインスペクター
    /// </summary>
    [CustomEditor(typeof(TweenParameterTrack))]
    public class TweenParameterTrackInspector : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset _inspectorXml;
        [SerializeField] private VisualTreeAsset _entryXml;
        private TweenParameterTrack _track;
        private ListView _listView;
        private List<EntryViewData> _viewDataList;

        /// <summary>
        /// ParameterSetEntryの表示用データ
        /// </summary>
        private class EntryViewData
        {
            public TweenParameterTrack.ParameterSetEntry BindingData;
        }

        private void OnEnable()
        {
            _track = (TweenParameterTrack)target;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        private void OnUndoRedoPerformed()
        {
            serializedObject.Update();
            GatherEntryViewData();
            _listView.RefreshItems();
        }

        /// <inheritdoc/>
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            _inspectorXml.CloneTree(root);
            _listView = root.Q<ListView>();

            _viewDataList ??= new();
            GatherEntryViewData();

            _listView.makeItem = _entryXml.CloneTree;
            _listView.bindItem = (elem, i) =>
            {
                var data = _viewDataList[i];
                var (listPropertyPath, listIndex) = _track.GetPropertyPath(data.BindingData.ParameterId);
                var property = serializedObject.FindProperty(listPropertyPath).GetArrayElementAtIndex(listIndex);
                (elem as IBindable)!.BindProperty(property);
                var typeEnumField = elem.Q<EnumField>();
                var paramType = _track.GetParameterType(data.BindingData);
                typeEnumField.SetValueWithoutNotify(TweenParameterEditorUtility.TypeToParameterType(paramType));
                typeEnumField.RegisterCallback<ChangeEvent<Enum>, EntryViewData>(OnParamTypeChange, data);
            };
            _listView.unbindItem = (elem, _) =>
            {
                var typeEnumField = elem.Q<EnumField>();
                typeEnumField.UnregisterCallback<ChangeEvent<Enum>, EntryViewData>(OnParamTypeChange);
            };
            _listView.itemsSource = _viewDataList;
            _listView.itemIndexChanged += (index1, index2) =>
            {
                Undo.RecordObject(_track, "Reorder Parameters");
                _viewDataList[index1].BindingData.ViewIndex = index1;
                _viewDataList[index2].BindingData.ViewIndex = index2;
            };

            var addButton = root.Q<Button>("add-button");
            addButton.clicked += () => AddItemAsync(addButton.GetScreenPosition()).Forget();
            var removeButton = root.Q<Button>("remove-button");
            removeButton.clicked += () => RemoveItem();

            return root;
        }

        /// <summary>
        /// Parameter追加
        /// </summary>
        private async UniTask AddItemAsync(Vector2 position)
        {
            var type = await EnumSearchWindow.OpenAsync<TweenParameterType>("Type", new SearchWindowContext(position));
            Undo.RecordObject(_track, "Add Tween Parameter");
            var bindingData = _track.AddEntry("New Parameter", TweenParameterEditorUtility.ParameterTypeToType(type));
            bindingData.ViewIndex = _viewDataList.Count == 0 ? 0 : _viewDataList[^1].BindingData.ViewIndex + 1;
            OnDataChanged();
            var viewData = new EntryViewData
            {
                BindingData = bindingData
            };
            _viewDataList.Add(viewData);
            _listView.RefreshItems();

            // focus added item
            _listView.selectedIndex = _viewDataList.Count - 1;
            _listView.GetSelectedItem().Q<TextField>().Focus();
        }

        /// <summary>
        /// Parameter削除
        /// </summary>
        private void RemoveItem()
        {
            Undo.RecordObject(_track, "Remove Tween Parameter");

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
            return;

            void remove(EntryViewData item)
            {
                _viewDataList.Remove(item);
                _track.RemoveEntry(item.BindingData.ParameterId);
            }
        }

        /// <summary>
        /// ParameterType変更時
        /// </summary>
        private void OnParamTypeChange(ChangeEvent<Enum> evt, EntryViewData data)
        {
            // データ側に反映
            Undo.RecordObject(_track, "Change Tween Parameter Type");
            var newType = (TweenParameterType)evt.newValue;
            var prevData = data.BindingData;
            var bindingData = _track.ConvertEntryType(prevData.ParameterId, TweenParameterEditorUtility.ParameterTypeToType(newType));
            bindingData.ViewIndex = prevData.ViewIndex;
            data.BindingData = bindingData;
            OnDataChanged();
            _listView.RefreshItems();
        }

        /// <summary>
        /// データ変更時
        /// </summary>
        private void OnDataChanged()
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            EditorUtility.SetDirty(target);
        }

        /// <summary>
        /// シリアライズされているデータを表示用データリストに格納
        /// </summary>
        private void GatherEntryViewData()
        {
            _viewDataList.Clear();
            AddEntries(_track.floats, nameof(_track.floats), TweenParameterType.Float);
            AddEntries(_track.ints, nameof(_track.ints), TweenParameterType.Int);
            AddEntries(_track.bools, nameof(_track.bools), TweenParameterType.Bool);
            AddEntries(_track.vector3s, nameof(_track.vector3s), TweenParameterType.Vector3);
            AddEntries(_track.vector2s, nameof(_track.vector2s), TweenParameterType.Vector2);
            AddEntries(_track.colors, nameof(_track.colors), TweenParameterType.Color);
            _viewDataList.Sort((a, b) => a.BindingData.ViewIndex.CompareTo(b.BindingData.ViewIndex));
            return;

            void AddEntries<T>(List<TweenParameterTrack.ParameterSetEntry<T>> entries, string listName, TweenParameterType parameterType)
            {
                if (entries == null) return;
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    _viewDataList.Add(new EntryViewData
                    {
                        BindingData = entry
                    });
                }
            }
        }
    }
}
