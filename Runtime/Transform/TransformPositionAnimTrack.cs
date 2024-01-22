using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    [DisplayName("Tween/Position Tween Anim Track")]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(TransformPositionAnimClip))]
    public class TransformPositionAnimTrack : TweenTrack
    {
        public override Tween CreateTween(CreateTweenArgs args)
        {
            return null;
        }
    }
}