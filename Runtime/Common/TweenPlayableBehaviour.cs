﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Playables;

namespace TweenTimeline
{
    [Serializable]
    public class TweenPlayableBehaviour : PlayableBehaviour
    {
        public TweenTimelineFieldOverride[] overrides;
        
        protected PlayableDirector _director { get; private set; }
        
        public override void OnPlayableCreate(Playable playable)
        {
            _director = (PlayableDirector)playable.GetGraph().GetResolver();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ApplyOverrides(TweenParameter parameter)
        {
            if (overrides == null) return;
            
            var fields = GatherFields(this);
            
            foreach (var fieldOverride in overrides)
            {
                if (fields.TryGetValue(fieldOverride.Name, out var field))
                {
                    fieldOverride.Expression?.Override(field, parameter);
                }
                // else
                // {
                //     Debug.LogWarning($"field {fieldOverride.Name} is not found.");
                // }
            }
        }
        
        /// <summary>
        /// TimelineFieldをDictionaryに入れる
        /// </summary>
        private static Dictionary<string, TweenTimelineField> GatherFields(TweenPlayableBehaviour behaviour)
        {
            var results = new Dictionary<string, TweenTimelineField>();
            results.Clear();

            // TODO: SourceGeneratorでやる
            var fields = behaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (!field.FieldType.IsSubclassOf(typeof(TweenTimelineField))) continue;
                results.Add(field.Name, (TweenTimelineField)field.GetValue(behaviour));
            }

            return results;
        }
    }
}