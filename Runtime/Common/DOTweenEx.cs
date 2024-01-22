using DG.Tweening;

namespace TweenTimeline
{
    /// <summary>
    /// DOTween拡張
    /// </summary>
    public static class DOTweenEx
    {
        /// <summary>
        /// 何もしないTweener
        /// </summary>
        public static Tweener Empty(float duration)
        {
            return DOTween.To(() => 0, _ => { }, 0, duration);
        }

        /// <summary>
        /// 一定時間何かをするTweener
        /// </summary>
        public static Tweener EveryUpdate(float duration, TweenCallback<float> onUpdate)
        {
            return DOVirtual.Float(0, duration, duration, onUpdate).SetEase(Ease.Linear);
        }

        /// <summary>
        /// Easeをセット
        /// </summary>
        public static T SetEase<T>(this T self, EaseOrCurve easeOrCurve) where T : Tween
        {
            if (easeOrCurve.UseAnimationCurve)
            {
                return self.SetEase(easeOrCurve.Curve);
            }
            else
            {
                return self.SetEase(easeOrCurve.Ease);
            }
        }
    }
}
