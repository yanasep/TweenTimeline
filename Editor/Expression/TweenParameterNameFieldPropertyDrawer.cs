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
        private string[] _paramNameOptions;
        // 空の選択肢を用意したいが、空文字やスペースだとPopupで区切り線として扱われてしまうため、見えない文字で代用
        private const string EmptyValue = "\u00A0";

        private void Initialize(SerializedProperty property)
        {
            if (_initialized) return;
            _initialized = true;

            GatherParameterNames(property);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Assert.IsTrue(property.propertyType == SerializedPropertyType.String);
            
            Initialize(property);
            
            var fieldPos = EditorGUI.PrefixLabel(position, label);
            var options = _paramNameOptions;
            var val = property.stringValue;
            if (string.IsNullOrEmpty(val)) val = EmptyValue;
            bool isMissing = false;
            var index = Array.IndexOf(options, val);
            if (index < 0)
            {
                isMissing = true;
                options = options.Prepend($"{property.stringValue} (missing)").ToArray();
                index = 0;
            }
            
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var optionGuis = options.Select(x => new GUIContent(x)).ToArray();
                if (isMissing)
                {
                    var content = EditorGUIUtility.IconContent("console.warnicon.sml");
                    content.text = options[0];
                    optionGuis[0] = content;
                }
                index = EditorGUI.Popup(fieldPos, index, optionGuis);

                if (ccs.changed)
                {
                    var newVal = options[index];
                    if (index == Array.IndexOf(options, EmptyValue)) newVal = "";
                    property.stringValue = newVal;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        private void GatherParameterNames(SerializedProperty property)
        {
            _paramNameOptions = Array.Empty<string>();
            if (property.serializedObject.context is not PlayableDirector director) return;
            if (director.playableAsset is not TimelineAsset timelineAsset) return;
            var parameterTrack = TweenTimelineUtility.FindTweenParameterTrack(timelineAsset);
            if (parameterTrack == null) return;
            var attr = (TweenParameterNameFieldAttribute)attribute;
            var paramType = TweenParameterEditorUtility.TypeToParameterType(attr.ParameterType);
            var paramList = TweenParameterEditorUtility.GetParameterSetEntries(parameterTrack, paramType);
            _paramNameOptions = paramList.Select(x => x.Name).Prepend(EmptyValue).ToArray();
        }
    }
}
