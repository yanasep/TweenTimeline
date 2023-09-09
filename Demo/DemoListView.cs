using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    public class DemoListView : MonoBehaviour
    {
        [SerializeField] private TweenTimelineDirector _director;
        [SerializeField] private Vector3 _targetPosition;

        private void Start()
        {
            _director.Initialize();
        }

        [EditorPlayModeButton("Play")]
        public void Play()
        {
            _director.ParameterContainer.Vector3.Set("TargetPosition", _targetPosition);
            _director.Play();
        }
    }
}
