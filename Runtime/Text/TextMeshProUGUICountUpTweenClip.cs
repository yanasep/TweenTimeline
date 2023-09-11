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
        protected override TweenBehaviour<TextMeshProUGUI> template => _behaviour;
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
        public override TweenCallback OnStartCallback => () =>
        {
            int.TryParse(Target.text, out _startValue);
            _endValue = EndValue.GetValue(Parameter);
        };

        /// <inheritdoc/>
        public override Tween GetTween()
        {
            return DOVirtual.Int(_startValue, _endValue, Duration, x => Target.SetText("{0}", x)).SetEase(Ease);
        }

        /// <inheritdoc/>
        public override TweenCallback OnEndCallback => () =>
        {
            Target.SetText("{0}", _endValue);
        };
    }
}