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
        [SerializeReference, SelectableSerializeReference] 
        private TimelineExpressionVector3 EndValue = new TimelineExpressionVector3Constant();
        
        [SerializeField] private Ease Ease;
        [SerializeField] private bool IsLocal;
        
        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<Transform> info)
        {
            if (IsLocal)
            {
                return info.Target.DOLocalRotate(EndValue.GetValue(info.Parameter), info.Duration, RotateMode.FastBeyond360).SetEase(Ease);
            }
            else
            {
                return info.Target.DORotate(EndValue.GetValue(info.Parameter), info.Duration, RotateMode.FastBeyond360).SetEase(Ease);
            }            
        }
    }
}