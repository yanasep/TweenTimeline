using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline.Editor
{
    [CustomPropertyDrawer(typeof(TweenParameterNameFieldAttribute), true)]
    public class TweenParameterNameFieldPropertyDrawer : PropertyDrawer
    {
        private bool _initialized;
        private string[] _paramNames;
        private GUIContent[] _options;
        private int _index;
        // 空の選択肢を用意したいが、空文字やスペースだとPopupで区切り線として扱われてしまうため、見えない文字で代用
        private const string EmptyValue = "\u00A0";

        private void Initialize(SerializedProperty property)
        {
            if (_initialized) return;
            _initialized = true;

            GatherParameterNames(property);
            UpdateOptions(property);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Assert.IsTrue(property.propertyType == SerializedPropertyType.String);
            
            Initialize(property);
            
            var fieldPos = EditorGUI.PrefixLabel(position, label);
            
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                // 警告アイコンは必ず表示させたいので、サイズを指定
                var prevSide = EditorGUIUtility.GetIconSize();
                EditorGUIUtility.SetIconSize(new Vector2(16, 16));
                
                _index = EditorGUI.Popup(fieldPos, _index, _options);
                
                EditorGUIUtility.SetIconSize(prevSide);

                if (ccs.changed)
                {
                    var newVal = _options[_index].text;
                    if (_index == Array.IndexOf(_options, EmptyValue)) newVal = "";
                    property.stringValue = newVal;
                    UpdateOptions(property);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        private void GatherParameterNames(SerializedProperty property)
        {
            _paramNames = Array.Empty<string>();
            if (property.serializedObject.context is not PlayableDirector director) return;
            if (director.playableAsset is not TimelineAsset timelineAsset) return;
            var parameterTrack = TweenTimelineUtility.FindTweenParameterTrack(timelineAsset);
            if (parameterTrack == null) return;
            var attr = (TweenParameterNameFieldAttribute)attribute;
            var paramType = TweenParameterEditorUtility.TypeToParameterType(attr.ParameterType);
            var paramList = TweenParameterEditorUtility.GetParameterSetEntries(parameterTrack, paramType);
            _paramNames = paramList.Select(x => x.Name).Prepend(EmptyValue).ToArray();
        }

        private void UpdateOptions(SerializedProperty property)
        {
            var val = property.stringValue;
            if (string.IsNullOrEmpty(val)) val = EmptyValue;
            _index = Array.IndexOf(_paramNames, val);
            bool isMissing = _index < 0;
            if (isMissing)
            {
                var missingContent = new GUIContent(EditorGUIUtility.IconContent("console.warnicon.sml"));
                missingContent.text = $"{property.stringValue} (missing)";
                
                _options = _paramNames.Select(x => new GUIContent(x))
                    .Prepend(missingContent)
                    .ToArray();

                _index = 0;
            }
            else
            {
                _options = _paramNames.Select(x => new GUIContent(x)).ToArray();
            }
        }
    }
}
