using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace TweenTimeline
{
    /// <summary>
    /// アクティベーションTweenトラック
    /// </summary>
    [DisplayName("Tween/Activation Track")]
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(ActivationTweenClip))]
    [TrackColor(0.2f, 1f, 0.2f)]
    public class ActivationTweenTrack : TweenTrack<GameObject>
    {
#if UNITY_EDITOR
        public override Texture2D Icon => EditorGUIUtility.IconContent("d_Toggle Icon").image as Texture2D;
#endif

        public override Tween CreateTween(CreateTweenArgs args)
        {
            var target = (GameObject)args.Binding;
            var inputs = GetClipInputs();
            return DOTweenEx.EveryUpdate(args.Duration, t =>
            {
                target.SetActive(inputs.IsAnyPlaying(t));
            });
        }

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            base.GatherProperties(director, driver);

#if UNITY_EDITOR
            var binding = director.GetGenericBinding(this) as Graphic;
            if (binding == null) return;
            driver.AddFromName(binding, "m_IsActive");
#endif
        }
    }
}
