using UnityEngine.Timeline;
using UnityEngine.UI;

namespace TweenTimeline
{
    /// <summary>
    /// GraphicTweenトラックのベース
    /// </summary>
    [TrackColor(1f, 1f, 1f)]
    public abstract class GraphicTweenTrack : TweenTrack<Graphic>
    {
    }
}
