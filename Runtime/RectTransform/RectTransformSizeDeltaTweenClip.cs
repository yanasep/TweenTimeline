using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// サイズTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Size Tween")]
    public class RectTransformSizeDeltaTweenClip : TweenClip<RectTransform>
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector2 EndValue = new TweenTimelineExpressionVector2Constant(new Vector2(100, 100));
        
        public EaseOrCurve Ease;
        
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionBool IsRelative = new TweenTimelineExpressionBoolConstant();
        
        public override Tween CreateTween(TweenClipInfo<RectTransform> info)
        {
            var target = info.Target;
            return DOTween.To(() => target.sizeDelta, x => target.sizeDelta = x,
                    EndValue.Evaluate(info.Parameter),
                    info.Duration)
                .SetEase(Ease)
                .SetRelative(IsRelative.Evaluate(info.Parameter));
        }
    }
}