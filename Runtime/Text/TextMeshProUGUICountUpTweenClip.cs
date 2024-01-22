using System;
using System.ComponentModel;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// TextMeshProUGUIの文字送りTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Text Count Up Tween")]
    public class TextMeshProUGUICountUpTweenClip : TweenClip<TextMeshProUGUI>
    {
        [SerializeReference, SelectableSerializeReference]
        public TweenTimelineExpressionInt EndValue = new TweenTimelineExpressionIntConstant();

        public EaseOrCurve Ease;

        /// <inheritdoc/>
        public override Tween CreateTween(TweenClipInfo<TextMeshProUGUI> info)
        {
            var start = int.TryParse(info.Target.text, out var val) ? val : 0;
            return DOVirtual.Int(start, EndValue.Evaluate(info.Parameter), info.Duration, val => info.Target.SetText("{0}", val))
                .SetEase(Ease);
        }
    }
}
