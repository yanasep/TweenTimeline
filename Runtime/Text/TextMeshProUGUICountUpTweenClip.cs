using System;
using System.ComponentModel;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Yanasep;

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
        private TimelineExpressionInt EndValue = new TimelineExpressionIntConstant { Value = 0 };
        
        [SerializeField] private Ease ease;

        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<TextMeshProUGUI> info)
        {
            return DOTween.To(() => int.TryParse(info.Target.text, out var val) ?  val : 0, x => info.Target.SetText("{0}", x), EndValue.GetValue(info.Parameter), info.Duration)
                .SetEase(ease);
        }
    }
}