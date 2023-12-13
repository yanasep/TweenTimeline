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
            tweenParam.SetColor("EndColor", Color.blue);
        });
    }
}
