using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// 移動Tweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Position Tween")]
    public class TransformPositionTweenClip : TweenClip<Transform>
    {
        public TransformTweenPositionType PositionType;

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionVector3 EndValue = new TweenTimelineExpressionVector3Constant();

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionBool IsRelative = new TweenTimelineExpressionBoolConstant();
        
        public EaseOrCurve Ease;

        /// <inheritdoc/>
        public override Tween CreateTween(TweenClipInfo<Transform> info)
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