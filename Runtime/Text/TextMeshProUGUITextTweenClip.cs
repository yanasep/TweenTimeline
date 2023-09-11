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
    [DisplayName("Text Tween")]
    public class TextMeshProUGUITextTweenClip : TweenClip<TextMeshProUGUI>
    {
        [SerializeField, ExtractContent] private TextMeshProUGUITextTweenBehaviour _behaviour;
        protected override TweenBehaviour<TextMeshProUGUI> template => _behaviour;
    }

    /// <summary>
    /// TextMeshProUGUIの文字送りTweenビヘイビア
    /// </summary>
    [Serializable]
    public class TextMeshProUGUITextTweenBehaviour : TweenBehaviour<TextMeshProUGUI>
    {
        public Ease Ease;

        private int _endValue;

        /// <inheritdoc/>
        public override TweenCallback OnStartCallback => () =>
        {
            _endValue = Target.text.Length;
        };

        /// <inheritdoc/>
        public override Tween GetTween()
        {
            return DOVirtual.Int(0, _endValue, Duration, x => Target.maxVisibleCharacters = x).SetEase(Ease);
        }

        /// <inheritdoc/>
        public override TweenCallback OnEndCallback => () =>
        {
            Target.maxVisibleCharacters = 99999;
        };
    }
}