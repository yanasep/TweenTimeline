using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yanasep;

namespace TweenTimeline
{
    [Serializable]
    public class SubTweenClip : TweenClip<PlayableDirector>
    {
        [SerializeField] private TimelineAsset _timelineAsset;
        [SerializeField] private ParameterOverwrite<TweenTimelineExpressionInt, int>[] ints;
        [SerializeField] private ParameterOverwrite<TweenTimelineExpressionFloat, float>[] floats;
        [SerializeField] private ParameterOverwrite<TweenTimelineExpressionBool, bool>[] bools;
        [SerializeField] private ParameterOverwrite<TweenTimelineExpressionVector3, Vector3>[] vector3s;
        [SerializeField] private ParameterOverwrite<TweenTimelineExpressionVector2, Vector2>[] vector2s;
        [SerializeField] private ParameterOverwrite<TweenTimelineExpressionColor, Color>[] colors;
        
        double m_Duration = PlayableBinding.DefaultDuration;
        bool m_SupportLoop;
        
        public override double duration => m_Duration;
        public override ClipCaps clipCaps => ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | (m_SupportLoop ? ClipCaps.Looping : ClipCaps.None);

        [Serializable]
        private struct ParameterOverwrite<TExpression, TValue> where TExpression : TweenTimelineExpression<TValue>
        {
            public string ParameterName;
            [SerializeReference, SelectableSerializeReference]
            public TExpression Expression;
        }
        
        public override Tween CreateTween(TweenClipInfo<PlayableDirector> info)
        {
            return TweenTimelineUtility.CreateTween(_timelineAsset, info.Target, parameter =>
            {
                Set(parameter.Int, ints, info.Parameter);
                Set(parameter.Float, floats, info.Parameter);
                Set(parameter.Bool, bools, info.Parameter);
                Set(parameter.Vector3, vector3s, info.Parameter);
                Set(parameter.Vector2, vector2s, info.Parameter);
                Set(parameter.Color, colors, info.Parameter);
            });
        }

        private void Set<TVal, TExp>(TimelineParameterDictionary<TVal> destParamDic, ParameterOverwrite<TExp, TVal>[] overwrites, 
            TweenParameter parentParameter)
            where TExp : TweenTimelineExpression<TVal>
        {
            foreach (var overwrite in overwrites)
            {
                destParamDic.Set(overwrite.ParameterName, overwrite.Expression.Evaluate(parentParameter));
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var director = target;
            // update the duration and loop values (used for UI purposes) here
            // so they are tied to the latest gameObject bound
            UpdateDurationAndLoopFlag(director);
            
            return base.CreatePlayable(graph, owner);
        }

        internal void UpdateDurationAndLoopFlag(PlayableDirector director)
        {
            if (director == null)
                return;

            const double invalidDuration = double.NegativeInfinity;

            var maxDuration = invalidDuration;
            var supportsLoop = false;

            if (director.playableAsset != null)
            {
                var assetDuration = director.playableAsset.duration;

                // if (director.playableAsset is TimelineAsset && assetDuration > 0.0)
                // {
                //     // Timeline assets report being one tick shorter than they actually are, unless they are empty
                //     assetDuration = (double)((DiscreteTime)assetDuration).OneTickAfter();
                // }

                maxDuration = Math.Max(maxDuration, assetDuration);
                supportsLoop = director.extrapolationMode == DirectorWrapMode.Loop;
            }

            m_Duration = double.IsNegativeInfinity(maxDuration) ? PlayableBinding.DefaultDuration : maxDuration;
            m_SupportLoop = supportsLoop;
        }
    }
}