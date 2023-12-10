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
        public EaseOrCurve Ease;

        /// <inheritdoc/>
        public override Tween CreateTween(TweenClipInfo<TextMeshProUGUI> info)
        {
            return DOVirtual.Int(0, info.Target.text.Length, info.Duration, val => info.Target.maxVisibleCharacters = val)
                .SetEase(Ease);
        }
    }
}