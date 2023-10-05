using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// TweenTimelineFieldの上書き設定
    /// </summary>
    [Serializable]
    public class TweenTimelineFieldOverwrite
    {
        /// <summary>フィールド名</summary>
        public string Name;

        /// <summary>オーバーライド</summary>
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverwriteExpression Expression;
    }
}
