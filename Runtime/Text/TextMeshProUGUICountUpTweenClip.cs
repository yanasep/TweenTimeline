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
        [SerializeField, ExtractContent] private TextMeshProUGUICountUpTweenBehaviour _behaviour;
        protected override TweenBehaviour<TextMeshProUGUI> Template => _behaviour;
    }

    /// <summary>
    /// TextMeshProUGUIの文字送りTweenビヘイビア
    /// </summary>
    [Serializable]
    public class TextMeshProUGUICountUpTweenBehaviour : TweenBehaviour<TextMeshProUGUI>
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionInt EndValue = new TimelineExpressionIntConstant { Value = 0 };
        
        public Ease Ease;

        private int _startValue;
        private int _endValue;

        /// <inheritdoc/>
        public override void Start()
        {
            int.TryParse(Target.text, out _startValue);
            _endValue = EndValue.GetValue(Parameter);
        }

        /// <inheritdoc/>
        public override void Update(float localTime)
        {
            Target.SetText("{0}", (int)DOVirtual.EasedValue(_startValue, _endValue, localTime / Duration, Ease));
        }

        /// <inheritdoc/>
        public override void End()
        {
            Target.SetText("{0}", _endValue);
        }
    }
}