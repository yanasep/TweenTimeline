// using System;
// using System.ComponentModel;
// using DG.Tweening;
// using TMPro;
// using UnityEngine;
//
// namespace TweenTimeline
// {
//     /// <summary>
//     /// TextMeshProUGUIの文字送りTweenクリップ
//     /// </summary>
//     [Serializable]
//     [DisplayName("Text Count Up Tween")]
//     public class TextMeshProUGUICountUpTweenClip : TweenClip<TextMeshProUGUI, TextMeshProUGUICountUpTweenBehaviour>
//     {
//     }
//
//     [Serializable]
//     public class TextMeshProUGUICountUpTweenBehaviour : TweenBehaviour<TextMeshProUGUI>
//     {
//         [SerializeField] private TweenTimelineFieldInt EndValue;
//         
//         [SerializeField] private TweenTimelineField<Ease> ease;
//
//         private int _start;
//
//         public override void Start()
//         {
//             _start = int.TryParse(Target.text, out var val) ? val : 0;
//         }
//
//         /// <inheritdoc/>
//         public override void Update(double localTime)
//         {
//             Target.SetText("{0}", DOVirtual.EasedValue(_start, EndValue.Value, (float)(localTime / Duration), ease.Value));
//         }   
//     }
// }