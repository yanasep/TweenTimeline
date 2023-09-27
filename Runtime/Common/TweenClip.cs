using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace TweenTimeline
{
    /// <summary>
    /// タイムラインTweenクリップ
    /// </summary>
    public abstract class TweenClip : PlayableAsset
    {
    }

    /// <summary>
    /// タイムラインTweenクリップ
    /// </summary>
    [Serializable]
    public abstract class TweenClip<TBinding> : TweenClip, ITimelineClipAsset where TBinding : Object
    {
        public virtual ClipCaps clipCaps => ClipCaps.None;

        // trackからセットされる
        public TBinding Binding { get; set; }
        public double StartTime { get; set; }
        public double Duration { get; set; }
        
        public TweenTimelineFieldOverride[] Overrides;
        public Dictionary<string, TweenTimelineField> Fields { get; set; }

        protected abstract Tween GetTween(TweenClipInfo<TBinding> info);
        public virtual TweenCallback GetStartCallback(TweenClipInfo<TBinding> info) => null;
        public virtual TweenCallback GetEndCallback(TweenClipInfo<TBinding> info) => null;
        public virtual string GetStartLog(TweenClipInfo<TBinding> info) => null;
        public virtual string GetTweenLog(TweenClipInfo<TBinding> info) => null;
        public virtual string GetEndLog(TweenClipInfo<TBinding> info) => null;

        /// <summary>
        /// Tweenを作成
        /// </summary>
        public Tween CreateTween(TweenClipInfo<TBinding> info)
        {
            GatherFields();
            ApplyOverrides(info.Parameter);
            return GetTween(info);
        }

        /// <inheritdoc/>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return default;
        }

        /// <summary>
        /// TimelineFieldをDictionaryに入れる
        /// </summary>
        private void GatherFields()
        {
            Fields ??= new();
            Fields.Clear();

            // TODO: SourceGeneratorでやる
            var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (!field.FieldType.IsSubclassOf(typeof(TweenTimelineField))) continue;
                Fields.Add(field.Name, (TweenTimelineField)field.GetValue(this));
            }
        }

        private void ApplyOverrides(TweenParameter parameter)
        {
            if (Overrides == null) return;
            foreach (var fieldOverride in Overrides)
            {
                if (Fields.TryGetValue(fieldOverride.Name, out var field))
                {
                    fieldOverride.Expression.Override(field, parameter);
                }
                else
                {
                    Debug.LogWarning($"{name}: field {fieldOverride.Name} is not found.");
                }
            }
        }
    }
}