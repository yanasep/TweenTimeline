using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline.Editor
{
    [CustomPropertyDrawer(typeof(TweenParameterIdFieldAttribute), true)]
    public class TweenParameterIdFieldPropertyDrawer : PropertyDrawer
    {
        private bool _initialized;
        private (uint paramId, string paramName)[] _parameters;
        private GUIContent[] _options;
        // optionsの各要素に対応するparameterId
        private uint[] _optionsParamIds;
        private int _index;
        // 空の選択肢を用意したいが、空文字やスペースだとPopupで区切り線として扱われてしまうため、見えない文字で代用
        private const string EmptyValue = "\u00A0";
        private GUIContent _missingContent;

        private void Initialize(SerializedProperty property)
        {
            if (_initialized) return;
            _initialized = true;
            
            _missingContent = new GUIContent(EditorGUIUtility.IconContent("console.warnicon.sml"));
            _missingContent.text = "(missing)";

            GatherParameters(property);
            UpdateOptions(property);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Assert.IsTrue(property.propertyType == SerializedPropertyType.Integer);
            
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
                    property.uintValue = _optionsParamIds[_index];
                    UpdateOptions(property);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        private void GatherParameters(SerializedProperty property)
        {
            _parameters = Array.Empty<(uint, string)>();
            if (property.serializedObject.context is not PlayableDirector director) return;
            if (director.playableAsset is not TimelineAsset timelineAsset) return;
            var parameterTrack = TweenTimelineUtility.FindTweenParameterTrack(timelineAsset);
            if (parameterTrack == null) return;
            var attr = (TweenParameterIdFieldAttribute)attribute;
            // _parameters = paramList.Select(x => (x.ParameterId, x.ParameterName)).Prepend((0u, EmptyValue)).ToArray();
            _parameters = parameterTrack.GetEntriesOfType(attr.ParameterType).Select(x => (x.ParameterId, x.ParameterName)).ToArray();
        }

        private void UpdateOptions(SerializedProperty property)
        {
            if (property.uintValue == 0)
            {
                _index = 0;
            }
            else
            {
                _index = Array.FindIndex(_parameters, x => x.paramId == property.uintValue);
            }

            bool isMissing = _index < 0;
            var optionsEnumerable = _parameters.Select(x => new GUIContent(x.paramName)).Prepend(new GUIContent(EmptyValue));
            var paramIdsEnumerable = _parameters.Select(x => x.paramId).Prepend(0u);
            
            if (isMissing)
            {
                _options = optionsEnumerable.Prepend(_missingContent).ToArray();
                _optionsParamIds = paramIdsEnumerable.Prepend(0u).ToArray();
                _index = 0;
            }
            else
            {
                _options = optionsEnumerable.ToArray();
                _optionsParamIds = paramIdsEnumerable.ToArray();
                // emptyValueを追加している分下げる
                _index++;
            }
        }
    }
}
