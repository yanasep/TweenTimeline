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
        public override Tween CreateTween(TweenClipInfo<GameObject> info)
        {
            return null;
        }
    }
}
