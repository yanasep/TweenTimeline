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
        private TimelineExpressionInt endValue = new TimelineExpressionIntConstant { Value = 0 };
        
        [SerializeField] private Ease ease;

        private int _startValue;
        private int _endValue;
        
        /// <inheritdoc/>
        public override TweenCallback GetStartCallback(TweenClipInfo<TextMeshProUGUI> info)
        {
            return () =>
            {
                int.TryParse(info.Target.text, out _startValue);
                _endValue = endValue.GetValue(info.Parameter);
            };
        }

        /// <inheritdoc/>
        public override Tween GetTween(TweenClipInfo<TextMeshProUGUI> info)
        {
            return DOVirtual.Int(_startValue, _endValue, info.Duration, x => info.Target.SetText("{0}", x)).SetEase(ease);
        }

        /// <inheritdoc/>
        public override TweenCallback GetEndCallback(TweenClipInfo<TextMeshProUGUI> info)
        {
            return () =>
            {
                info.Target.SetText("{0}", _endValue);
            };
        }
    }
}