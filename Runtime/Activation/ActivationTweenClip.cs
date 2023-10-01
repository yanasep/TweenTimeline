using System;
using System.ComponentModel;
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
        protected override TweenBehaviour<GameObject> Template => null;
    }
}
