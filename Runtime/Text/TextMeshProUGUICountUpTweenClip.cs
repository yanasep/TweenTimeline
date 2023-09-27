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
        [SerializeField] private TweenTimelineField<int> EndValue;
        
        [SerializeField] private TweenTimelineField<Ease> ease;

        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<TextMeshProUGUI> info)
        {
            return DOTween.To(() => int.TryParse(info.Target.text, out var val) ?  val : 0, x => info.Target.SetText("{0}", x), EndValue.Value, info.Duration)
                .SetEase(ease.Value);
        }
    }
}