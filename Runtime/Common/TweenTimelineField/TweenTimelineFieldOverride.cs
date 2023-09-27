using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// TweenTimelineFieldのオーバーライド設定
    /// </summary>
    [Serializable]
    public class TweenTimelineFieldOverride
    {
        /// <summary>フィールド名</summary>
        public string Name;

        /// <summary>オーバーライド</summary>
        [SerializeReference, SelectableSerializeReference]
        public TimelineOverrideExpression Expression;
    }
}
