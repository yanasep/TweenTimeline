using TweenTimeline;
using UnityEngine;
using Yanasep;

public class ImageTweenDemo : MonoBehaviour
{
    [SerializeField] private TweenTimelineDirector director;

    [EditorButton("Play")]
    public void Play()
    {
        director.Play(tweenParam =>
        {
            tweenParam.Color.Set("EndColor", Color.blue);
        });
    }
}
