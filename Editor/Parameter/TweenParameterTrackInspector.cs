using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TweenTimeline
{
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
            public string Name;
            public string BindingListName;
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
            };
            _listView.itemsSource = paramList;
            _listView.itemIndexChanged += (index1, index2) =>
            {
                paramList[index1].BindingData.ViewIndex = index1;
                paramList[index2].BindingData.ViewIndex = index2;
            };

            return root;
        }

        private void GatherParamListData()
        {
            paramList.Clear();
            AddEntries(_track.floats, nameof(_track.floats));
            AddEntries(_track.ints, nameof(_track.ints));
            AddEntries(_track.bools, nameof(_track.bools));
            AddEntries(_track.vector3s, nameof(_track.vector3s));
            AddEntries(_track.vector2s, nameof(_track.vector2s));
            AddEntries(_track.colors, nameof(_track.colors));
            return;
            
            void AddEntries<T>(TweenParameterTrack.ParameterSetEntry<T>[] entries, string listName)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    paramList.Add(new ListItem
                    {
                        Name = entry.Name,
                        BindingListName = listName,
                        BindingListItemIndex = i,
                        BindingData = entry
                    });
                }
            }   
        }
    }
}