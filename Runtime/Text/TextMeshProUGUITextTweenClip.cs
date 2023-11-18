// using System;
// using System.ComponentModel;
// using DG.Tweening;
// using TMPro;
//
// namespace TweenTimeline
// {
//     /// <summary>
//     /// TextMeshProUGUIの文字送りTweenクリップ
//     /// </summary>
//     [Serializable]
//     [DisplayName("Text Tween")]
//     public class TextMeshProUGUITextTweenClip : TweenClip<TextMeshProUGUI, TextMeshProUGUITextTweenBehaviour>
//     {
//     }
//
//     [Serializable]
//     public class TextMeshProUGUITextTweenBehaviour : TweenBehaviour<TextMeshProUGUI>
//     {
//         public Ease ease;
//
//         /// <inheritdoc/>
//         public override void Update(double localTime)
//         {
//             Target.maxVisibleCharacters = (int)DOVirtual.EasedValue(0, Target.text.Length, (float)(localTime / Duration), ease);
//         }
//
//         public override void End()
//         {
//             Target.maxVisibleCharacters = 99999;
//         }
//     }
// }