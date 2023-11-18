// using System;
// using System.ComponentModel;
// using DG.Tweening;
// using UnityEngine;
//
// namespace TweenTimeline
// {
//     /// <summary>
//     /// 移動Tweenクリップ
//     /// </summary>
//     [Serializable]
//     [DisplayName("Position Tween")]
//     public class TransformPositionTweenClip : TweenClip<Transform, TransformPositionTweenBehaviour>
//     {
//     }
//
//     [Serializable]
//     public class TransformPositionTweenBehaviour : TweenBehaviour<Transform>
//     {
//         public TweenTimelineField<TransformTweenPositionType> PositionType;
//
//         public TweenTimelineFieldVector3 EndValue;
//
//         public TweenTimelineFieldBool IsRelative;
//         
//         public TweenTimelineField<Ease> Ease;
//
//         private Vector3 _startValue;
//
//         public override void Start()
//         {
//             _startValue = Target.GetPosition(PositionType.Value);
//         }
//
//         /// <inheritdoc/>
//         public override void Update(double localTime)
//         {
//             var val = DOVirtual.EasedValue(_startValue, EndValue.Value, (float)(localTime / Duration), Ease.Value);
//             if (IsRelative.Value) val += _startValue;
//             Target.SetPosition(PositionType.Value, val);
//         }
//     }
// }