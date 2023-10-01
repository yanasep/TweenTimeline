using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yanasep;
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
        public TweenParameter Parameter { get; set; }
        
        public TweenTimelineFieldOverride[] Overrides;
        public Dictionary<string, TweenTimelineField> Fields { get; set; }

        // protected abstract TweenBehaviour<TBinding> Template { get; }
        protected virtual TweenBehaviour<TBinding> Template => null;

        protected virtual Tween GetTween(TweenClipInfo<TBinding> info) => null;
        public virtual TweenCallback GetStartCallback(TweenClipInfo<TBinding> info) => null;
        public virtual TweenCallback GetEndCallback(TweenClipInfo<TBinding> info) => null;
        public virtual string GetStartLog(TweenClipInfo<TBinding> info) => null;
        public virtual string GetTweenLog(TweenClipInfo<TBinding> info) => null;
        public virtual string GetEndLog(TweenClipInfo<TBinding> info) => null;

        /// <inheritdoc/>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            if (Binding == null || Template == null) return default;
            
            GatherFields();
            ApplyOverrides(Parameter);
            
            // OnCreatePlayableで参照できるようにtemplateにセット
            Template.Target = Binding;
            Template.StartTime = StartTime;
            Template.Duration = Duration;
            
            return ScriptPlayable<TweenBehaviour<TBinding>>.Create(graph, Template);
        }

        /// <summary>
        /// TimelineFieldをDictionaryに入れる
        /// </summary>
        private void GatherFields()
        {
            Fields ??= new();
            Fields.Clear();

            // TODO: SourceGeneratorでやる
            var fields = Template.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (!field.FieldType.IsSubclassOf(typeof(TweenTimelineField))) continue;
                Fields.Add(field.Name, (TweenTimelineField)field.GetValue(Template));
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

    /// <summary>
    /// タイムラインTweenクリップ
    /// </summary>
    [Serializable]
    public abstract class TweenClip<TBinding, TBehaviour> : TweenClip<TBinding> 
        where TBinding : Object
        where TBehaviour : TweenBehaviour<TBinding>
    {
        [SerializeField, ExtractContent] private TBehaviour template;
        protected override TweenBehaviour<TBinding> Template => template;
    }
    
    /// <summary>
    /// TweenTimelineのクリップのBehaviourのベースクラス
    /// </summary>
    public class TweenBehaviour : PlayableBehaviour
    {   
        public double StartTime { get; set; }
        public double Duration { get; set; }
        
        /// <summary>
        /// クリップ突入時
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// クリップ再生中の更新
        /// </summary>
        public virtual void Update(double localTime) { }

        /// <summary>
        /// クリップ終了時
        /// </summary>
        public virtual void End() { }
    }

    /// <summary>
    /// TweenTimelineのクリップのBehaviourのベースクラス
    /// </summary>
    public class TweenBehaviour<TBinding> : TweenBehaviour where TBinding : Object
    {
        public TBinding Target { get; set; }

        /// <inheritdoc/>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            Start();
        }

        /// <inheritdoc/>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);

            float t = (float)playable.GetTime();
            Update(t);
        }

        /// <inheritdoc/>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (info.evaluationType == FrameData.EvaluationType.Playback)
            {
                End();
            }
        }
    }
}