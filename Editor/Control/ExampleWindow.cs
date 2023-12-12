using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

public class ExampleWindow : EditorWindow
{
    private ReorderableList reorderableList;
    private List<EntryViewData> itemList = new();

    private class EntryViewData
    {
        public string BindingListName;
        public int BindingListItemIndex;
    }

    [MenuItem("Window/Example")]
    public static void ShowWindow()
    {
        GetWindow<ExampleWindow>("Example");
    }

    private void OnEnable()
    {
        reorderableList = new ReorderableList(itemList, typeof(EntryViewData), true, true, true, true);
        reorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Items");
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            if (index < itemList.Count)
            {
                var data = itemList[index];
                var nameRect = rect;
                nameRect.height = EditorGUIUtility.singleLineHeight;
                data.BindingListName = EditorGUI.TextField(nameRect, data.BindingListName);
                var indexRect = rect;
                indexRect.yMin = nameRect.yMax + EditorGUIUtility.standardVerticalSpacing;
                data.BindingListItemIndex = EditorGUI.IntField(indexRect, data.BindingListItemIndex);
            }
        };

        reorderableList.onAddCallback = (ReorderableList list) =>
        {
            itemList.Add(new EntryViewData
            {
                BindingListName = "New Item",
                BindingListItemIndex = 0
            });
        };
    }

    private void OnGUI()
    {
        reorderableList.DoLayoutList();
    }
}
