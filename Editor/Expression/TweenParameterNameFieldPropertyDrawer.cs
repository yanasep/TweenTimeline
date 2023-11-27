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
            var index = Array.IndexOf(options, property.stringValue);
            if (index < 0)
            {
                options = options.Prepend($"{property.stringValue} (missing)").ToArray();
            }
            
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                index = EditorGUI.Popup(fieldPos, index, options);

                if (ccs.changed)
                {
                    property.stringValue = options[index];
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
            _paramNameOptions = paramList.Select(x => x.Name).Prepend("").ToArray();
        }
    }
}
