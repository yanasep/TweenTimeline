using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;

namespace TweenTimeline
{
    [Serializable]
    public class TweenPlayableBehaviour : PlayableBehaviour
    {
        protected PlayableDirector _director { get; private set; }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            
            _director = (PlayableDirector)playable.GetGraph().GetResolver();
            var paramHolder = _director.gameObject.GetComponent<RuntimeTweenParameterHolder>();
            if (paramHolder == null)
            {
                Debug.LogWarning($"Please Add Tween Setup Track");
            }
            else
            {
                SetField(paramHolder.Parameter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetField(TweenParameter parameter)
        {
            var fields = GatherFields(this);
            
            foreach (var field in fields)
            {
                field.SetValue(parameter);
            }
        }
        
        /// <summary>
        /// TimelineFieldをDictionaryに入れる
        /// </summary>
        private static List<ITweenTimelineField> GatherFields(TweenPlayableBehaviour behaviour)
        {
            var results = new List<ITweenTimelineField>();
            results.Clear();

            // TODO: SourceGeneratorでやる
            var fieldInfos = behaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                if (!typeof(ITweenTimelineField).IsAssignableFrom(fieldInfo.FieldType)) continue;
                var tweenField = (ITweenTimelineField)fieldInfo.GetValue(behaviour);
                // クリップからBehaviour作成時にディーブコピー
                tweenField = tweenField.Clone();
                fieldInfo.SetValue(behaviour, tweenField);
                results.Add(tweenField);
            }

            return results;
        }
    }
}