using DG.Tweening;

namespace TweenTimeline
{
    public static class DOTweenEx
    {
        public static Tweener Empty(float duration)
        {
            return DOTween.To(() => 0, _ => { }, 0, duration);
        }

        public static Tweener EveryUpdate(float duration, TweenCallback<float> onUpdate)
        {
            return DOVirtual.Float(0, duration, duration, onUpdate).SetEase(Ease.Linear);
        }
    }
}
