using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// サイズTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Size Tween")]
    public class RectTransformSizeDeltaTweenClip : TweenClip<RectTransform>
    {
        [SerializeField, ExtractContent] private RectTransformSizeDeltaTweenBehaviour _behaviour;
        protected override TweenBehaviour<RectTransform> Template => _behaviour;
    }

    /// <summary>
    /// サイズTweenビヘイビア
    /// </summary>
    [Serializable]
    public class RectTransformSizeDeltaTweenBehaviour : TweenBehaviour<RectTransform>
    {
        [SerializeReference, SelectableSerializeReference]
        public TimelineExpressionVector2 EndValue = new TimelineExpressionVector2Constant { Value = new Vector2(100, 100) };
        public Ease Ease;

        private Tween _tween;

        /// <inheritdoc/>
        public override void Start()
        {
            _tween = Target.DOSizeDelta(EndValue.GetValue(Parameter), Duration).SetEase(Ease).SetUpdate(UpdateType.Manual);
        }

        /// <inheritdoc/>
        public override void Update(float localTime)
        {
            _tween.Goto(localTime);
        }
    }
}