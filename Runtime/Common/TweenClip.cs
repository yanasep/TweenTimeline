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

        private readonly TweenBehaviour template = new();

        /// <summary>
        /// Tweenを作成
        /// </summary>
        public Tween CreateTween(TweenClipInfo<TBinding> info)
        {
            Fields ??= new();
            Fields.Clear();
            GatherFields();

            foreach (var fieldOverride in Overrides)
            {
                if (Fields.TryGetValue(fieldOverride.Name, out var field))
                {
                    fieldOverride.Expression.Override(field, info.Parameter);
                }
                else
                {
                    Debug.LogWarning($"{name}: field {fieldOverride.Name} is not found.");
                }
            }

            return GetTween(info);
        }

        /// <summary>
        /// TimelineFieldをDictionaryに入れる
        /// </summary>
        private void GatherFields()
        {
            // TODO: SourceGeneratorでやる
            var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (!field.FieldType.IsSubclassOf(typeof(TweenTimelineField))) continue;
                Fields.Add(field.Name, (TweenTimelineField)field.GetValue(this));
            }
        }

        /// <inheritdoc/>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            if (Binding == null) return default;
            
            var parameterHolder = owner.GetComponent<TweenParameterHolder>();
            var parameter = parameterHolder != null ? parameterHolder.GetParameter() : new TweenParameter();

            var info = new TweenClipInfo<TBinding>
            {
                Target = Binding,
                Duration = (float)Duration,
                Parameter = parameter
            };
            
            // OnPlayableCreateでアクセスできるように予めtemplateにセット
            template.Tween = CreateTween(info);
            template.StartCallback = GetStartCallback(info);
            template.EndCallback = GetEndCallback(info);
            template.StartTime = StartTime;
            template.Duration = duration;
            return ScriptPlayable<TweenBehaviour>.Create(graph, template);
        }
    }

    /// <summary>
    /// TweenTimelineのクリップのBehaviourのベースクラス
    /// </summary>
    public class TweenBehaviour : PlayableBehaviour
    {
        public double StartTime { get; set; }
        public double Duration { get; set; }
        public Tween Tween { get; set; }
        public TweenCallback StartCallback { get; set; }
        public TweenCallback EndCallback { get; set; }

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
            Update(playable.GetTime());
        }

        /// <inheritdoc/>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);
            if (info.evaluationType == FrameData.EvaluationType.Playback)
            {
                End();
            }
        }

        /// <summary>
        /// クリップ突入時
        /// </summary>
        public void Start()
        {
            StartCallback?.Invoke();
        }

        /// <summary>
        /// クリップ再生中の更新
        /// </summary>
        public void Update(double localTime)
        {
            Tween.GotoWithCallbacks((float)localTime);
        }

        /// <summary>
        /// クリップ終了時
        /// </summary>
        public void End()
        {
            EndCallback?.Invoke();
        }
    }
}