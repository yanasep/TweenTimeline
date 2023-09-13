using System;
using System.ComponentModel;
using DG.Tweening;
using TMPro;

namespace TweenTimeline
{
    /// <summary>
    /// TextMeshProUGUIの文字送りTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Text Tween")]
    public class TextMeshProUGUITextTweenClip : TweenClip<TextMeshProUGUI>
    {
        public Ease ease;

        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<TextMeshProUGUI> info)
        {
            return DOTween.To(() => info.Target.text.Length, x => info.Target.maxVisibleCharacters = x, 0, info.Duration)
                .From().SetEase(ease);   
        }
    }
}