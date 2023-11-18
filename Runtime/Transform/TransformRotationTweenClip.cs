// using System;
// using System.ComponentModel;
// using DG.Tweening;
// using UnityEngine;
// using UnityEngine.Playables;
//
// namespace TweenTimeline
// {
//     /// <summary>
//     /// 回転Tweenクリップ
//     /// </summary>
//     [Serializable]
//     [DisplayName("Rotation Tween")]
//     public class TransformRotationTweenClip : TweenClip<Transform, TransformRotationTweenBehaviour>
//     {
//     }
//
//     [Serializable]
//     public class TransformRotationTweenBehaviour : TweenBehaviour<Transform>
//     {
//         [SerializeField] private TweenTimelineFieldVector3 EndValue;
//         
//         [SerializeField] private TweenTimelineField<Ease> Ease;
//         [SerializeField] private TweenTimelineFieldBool IsLocal;
//         
//         private Tween _tween;
//         
//         public override void Start()
//         {
//             if (IsLocal.Value)
//             {
//                 _tween = Target.DOLocalRotate(EndValue.Value, (float)Duration, RotateMode.FastBeyond360).SetEase(Ease.Value);
//             }
//             else
//             {
//                 _tween = Target.DORotate(EndValue.Value, (float)Duration, RotateMode.FastBeyond360).SetEase(Ease.Value);
//             }
//
//             _tween.Pause().SetAutoKill(false);
//         }
//
//         public override void Update(double localTime)
//         {
//             _tween.Goto((float)localTime);
//         }
//
//         public override void OnGraphStop(Playable playable)
//         {
//             base.OnGraphStop(playable);
//             _tween.Kill();
//         }
//     }
// }