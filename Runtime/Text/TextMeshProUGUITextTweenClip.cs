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
        protected override TweenBehaviour<TextMeshProUGUI> Template => _behaviour;
    }

    /// <summary>
    /// TextMeshProUGUIの文字送りTweenビヘイビア
    /// </summary>
    [Serializable]
    public class TextMeshProUGUITextTweenBehaviour : TweenBehaviour<TextMeshProUGUI>
    {
        public Ease Ease;

        private int _endValue;

        public override Tween GetTween()
        {
            throw new NotImplementedException();
        }

        // /// <inheritdoc/>
        // public override void Start()
        // {
        //     _endValue = Target.text.Length;
        // }
        //
        // /// <inheritdoc/>
        // public override void Update(float localTime)
        // {
        //     Target.maxVisibleCharacters = (int)DOVirtual.EasedValue(0, _endValue, localTime / Duration, Ease);
        // }
        //
        // /// <inheritdoc/>
        // public override void End()
        // {
        //     Target.maxVisibleCharacters = 99999;
        // }
    }
}