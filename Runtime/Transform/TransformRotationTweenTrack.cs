// using System;
// using System.ComponentModel;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.Playables;
// using UnityEngine.Timeline;
// using Yanasep;
//
// namespace TweenTimeline
// {
//     /// <summary>
//     /// 回転Tweenトラック
//     /// </summary>
//     [DisplayName("Tween/Rotation Tween Track")]
//     [TrackBindingType(typeof(RectTransform))]
//     [TrackClipType(typeof(TransformRotationTweenClip))]
//     public class TransformRotationTweenTrack : TransformTweenTrack<TransformRotationTweenMixerBehaviour>
//     {   
// #if UNITY_EDITOR
//         public override Texture2D Icon => EditorGUIUtility.IconContent("RotateTool").image as Texture2D;  
// #endif
//     }
//     
//     [Serializable]
//     public class TransformRotationTweenMixerBehaviour : TweenMixerBehaviour<Transform>
//     {
//         [SerializeField] private bool setStartValue;
//
//         [EnableIf(nameof(setStartValue), true)]
//         [SerializeField] private TweenTimelineFieldVector3 startValue;
//         
//         /// <inheritdoc/>
//         protected override void OnStart(Playable playable)
//         {
//             base.OnStart(playable);
//             if (!setStartValue) return;
//             Target.localRotation = Quaternion.Euler(startValue.Value);
//         }
//     }
// }