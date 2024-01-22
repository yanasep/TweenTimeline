using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    [Serializable]
    [DisplayName("Position Tween")]
    public class TransformPositionAnimClip : PlayableAsset, ITimelineClipAsset
    {
        public TransformPositionAnimBehaviour Behaviour;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<TransformPositionAnimBehaviour>.Create(graph, Behaviour);
        }

        public ClipCaps clipCaps { get; }
    }
    
    [Serializable]
    public class TransformPositionAnimBehaviour : PlayableBehaviour
    {
        public Vector3 Position;
    }
}