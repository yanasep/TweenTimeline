// using System;
// using System.ComponentModel;
// using UnityEngine;
// using Yanasep;
//
// namespace TweenTimeline
// {
//     /// <summary>
//     /// 移動Tweenクリップ
//     /// </summary>
//     [Serializable]
//     [DisplayName("Position Keep")]
//     public class TransformPositionKeepClip : TweenClip<Transform, TransformPositionKeepBehaviour>
//     {
//     }
//     
//     [Serializable]
//     public class TransformPositionKeepBehaviour : TweenBehaviour<Transform>
//     {
//         public TweenTimelineField<TransformTweenPositionType> PositionType;
//
//         public bool SpecifyValue;
//
//         [EnableIf(nameof(SpecifyValue), true)]
//         public TweenTimelineFieldVector3 Value;
//
//         private Vector3 _startValue;
//
//         /// <inheritdoc/>
//         public override void Start()
//         {
//             _startValue = SpecifyValue switch
//             {
//                 true => Value.Value,
//                 false => Target.GetPosition(PositionType.Value)
//             };
//         }
//
//         /// <inheritdoc/>
//         public override void Update(double localTime)
//         {
//             Target.SetPosition(PositionType.Value, _startValue);
//         }
//     }
// }