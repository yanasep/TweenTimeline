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

        private int endValue;
        
        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(TweenClipInfo<TextMeshProUGUI> info)
        {
            return () => endValue = info.Target.text.Length;
        }

        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<TextMeshProUGUI> info)
        {
            return DOVirtual.Int(0, endValue, info.Duration, x => info.Target.maxVisibleCharacters = x).SetEase(ease);   
        }

        /// <inheritdoc/>
        public override TweenCallback GetEndCallback(TweenClipInfo<TextMeshProUGUI> info)
        {
            return () => info.Target.maxVisibleCharacters = 99999;
        }
    }
}