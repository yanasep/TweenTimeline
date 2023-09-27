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

        /// <inheritdoc/>
        protected override TweenCallback GetStartCallback(TweenTrackInfo<GameObject> info)
        {
            return () => info.Target.SetActive(false);
        }

        /// <inheritdoc/>
        protected override string GetStartLog(TweenTrackInfo<GameObject> info)
        {
            return "GameObject.SetActive(false)";
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

        public override Tween CreateTween(CreateTweenArgs args)
        {
            var inputs = GetClipInputs();
            
            var go = (GameObject)args.Binding;
            return DOTweenEx.EveryUpdate((float)timelineAsset.duration, t =>
            {
                if (go == null) return;
                
                go.SetActive(IsAnyClipPlaying(inputs, t));
            });
        }
    }
}
