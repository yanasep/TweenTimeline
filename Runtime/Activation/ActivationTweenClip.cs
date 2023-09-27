using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// アクティベーションTweenクリップ
    /// </summary>
    [Serializable]
    [DisplayName("Activation Tween")]
    public class ActivationTweenClip : TweenClip<GameObject>
    {
        /// <inheritdoc/>
        protected override Tween GetTween(TweenClipInfo<GameObject> info)
        {
            return null;
        }
    }
}
