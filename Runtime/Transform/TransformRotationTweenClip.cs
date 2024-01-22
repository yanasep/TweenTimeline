using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// 回転Tweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Rotation Tween")]
    public class TransformRotationTweenClip : TweenClip<Transform>
    {
        [SerializeReference, SelectableSerializeReference]
        private TweenTimelineExpressionVector3 EndValue = new TweenTimelineExpressionVector3Constant();

        [SerializeField] private EaseOrCurve Ease;

        [SerializeReference, SelectableSerializeReference]
        private TweenTimelineExpressionBool IsLocal = new TweenTimelineExpressionBoolConstant();

        /// <inheritdoc/>
        public override Tween CreateTween(TweenClipInfo<Transform> info)
        {
            if (IsLocal.Evaluate(info.Parameter))
            {
                return info.Target.DOLocalRotate(EndValue.Evaluate(info.Parameter), info.Duration, RotateMode.FastBeyond360)
                    .SetEase(Ease);
            }
            else
            {
                return info.Target.DORotate(EndValue.Evaluate(info.Parameter), info.Duration, RotateMode.FastBeyond360).SetEase(Ease);
            }
        }
    }
}
