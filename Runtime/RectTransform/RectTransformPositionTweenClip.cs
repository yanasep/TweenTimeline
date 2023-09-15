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
        public TimelineExpressionVector3 EndValue = new TimelineExpressionVector3Constant();

        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionBool IsRelative = new TimelineExpressionBoolConstant { Value = false };
        
        public Ease Ease;
        
        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<RectTransform> info)
        {
            Tween tween;
            switch (PositionType)
            {
                case RectTransformTweenPositionType.AnchoredPosition:
                    tween = info.Target.DOAnchorPos(EndValue.GetValue(info.Parameter), info.Duration);
                    break;
                case RectTransformTweenPositionType.Position:
                    tween = info.Target.DOMove(EndValue.GetValue(info.Parameter), info.Duration);
                    break;
                case RectTransformTweenPositionType.LocalPosition:
                    tween = info.Target.DOLocalMove(EndValue.GetValue(info.Parameter), info.Duration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            tween.SetEase(Ease).SetUpdate(UpdateType.Manual);
            if (IsRelative.GetValue(info.Parameter)) tween.SetRelative(true);

            return tween;
        }

        /// <inheritdoc/>
        public override string GetTweenLog(TweenClipInfo<RectTransform> info)
        {
            string log;
            
            switch (PositionType)
            {
                case RectTransformTweenPositionType.AnchoredPosition:
                    log = $"DOAnchorPos({EndValue.GetValue(info.Parameter)}, {info.Duration})";
                    break;
                case RectTransformTweenPositionType.Position:
                    log = $"DOMove({EndValue.GetValue(info.Parameter)}, {info.Duration})";
                    break;
                case RectTransformTweenPositionType.LocalPosition:
                    log = $"DOLocalMove({EndValue.GetValue(info.Parameter)}, {info.Duration})";
                    break;
                default:
                    return null;
            }

            if (IsRelative.GetValue(info.Parameter)) log += ".SetRelative(true)";
            return log;
        }
    }
}
