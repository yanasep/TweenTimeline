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
    }
}