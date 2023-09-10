using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// 回転Tweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Rotation Tween")]
    public class TransformRotationTweenClip : TweenClip<Transform>
    {
        [SerializeField, ExtractContent] private TransformRotationTweenBehaviour _behaviour;
        protected override TweenBehaviour<Transform> Template => _behaviour;
    }

    /// <summary>
    /// 回転Tweenビヘイビア
    /// </summary>
    [Serializable]
    public class TransformRotationTweenBehaviour : TweenBehaviour<Transform>
    {
        [SerializeReference, SelectableSerializeReference] 
        public TimelineExpressionVector3 EndValue = new TimelineExpressionVector3Constant();
        
        public Ease Ease;
        public bool IsLocal;

        /// <inheritdoc/>
        public override Tween GetTween()
        {
            if (IsLocal)
            {
                return Target.DOLocalRotate(EndValue.GetValue(Parameter), Duration, RotateMode.FastBeyond360).SetEase(Ease);
            }
            else
            {
                return Target.DORotate(EndValue.GetValue(Parameter), Duration, RotateMode.FastBeyond360).SetEase(Ease);
            }
        }
    }
}