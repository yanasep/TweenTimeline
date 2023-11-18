using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// 移動Tweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Position Tween")]
    public class RectTransformPositionTweenClip : TweenClip<RectTransform>
    {
        public RectTransformTweenPositionType PositionType;
        
        [SerializeReference, SelectableSerializeReference] 
        public TweenTimelineExpressionVector3 EndValue;
        
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionBool IsRelative;
        
        public EaseOrCurve Ease;
        
        public override Tween CreateTween(TweenClipInfo<RectTransform> info)
        {
            var target = info.Target;
            return DOTween.To(() => target.GetPosition(PositionType), x => target.SetPosition(PositionType, x),
                    EndValue.Evaluate(info.Parameter),
                    info.Duration)
                .SetEase(Ease)
                .SetRelative(IsRelative.Evaluate(info.Parameter));
        }
    }
}
