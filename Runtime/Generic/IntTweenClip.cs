using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace TweenTimeline
{
    /// <summary>
    /// Int Tweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Int Tween")]
    public class IntTweenClip : TweenClipNoBinding
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionInt StartValue = new TweenTimelineExpressionIntConstant();

        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionInt EndValue = new TweenTimelineExpressionIntConstant();

        public EaseOrCurve Ease;

        public UnityEvent<int> OnUpdate;

        /// <inheritdoc/>
        public override Tween CreateTween(TweenClipInfo info)
        {
            return DOVirtual.Int(StartValue.Evaluate(info.Parameter), EndValue.Evaluate(info.Parameter), info.Duration, x => OnUpdate.Invoke(x)).SetEase(Ease);
        }
    }
}
