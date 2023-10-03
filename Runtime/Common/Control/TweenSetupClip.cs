using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace TweenTimeline
{
    /// <summary>
    /// Tweenセットアップクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Tween Setup")]
    public class TweenSetupClip : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return default;
        }
    }
}