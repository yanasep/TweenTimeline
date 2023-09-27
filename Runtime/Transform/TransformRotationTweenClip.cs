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
        [SerializeField] private TweenTimelineField<Vector3> EndValue;
        
        [SerializeField] private TweenTimelineField<Ease> Ease;
        [SerializeField] private TweenTimelineField<bool> IsLocal;
        
        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<Transform> info)
        {
            if (IsLocal.Value)
            {
                return info.Target.DOLocalRotate(EndValue.Value, info.Duration, RotateMode.FastBeyond360).SetEase(Ease.Value);
            }
            else
            {
                return info.Target.DORotate(EndValue.Value, info.Duration, RotateMode.FastBeyond360).SetEase(Ease.Value);
            }            
        }
    }
}