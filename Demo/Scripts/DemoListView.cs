using System;
using System.Collections.Generic;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    public class DemoListView : MonoBehaviour
    {
        [SerializeField] private TweenTimelineDirector _director;
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private List<CountryResult> _results;
        [SerializeField] private DemoListElement _elementTemplate;
        private Dictionary<CountryCode, DemoListElement> _elements;
        
        [Serializable]
        private class CountryResult
        {
            public CountryCode CountryCode;
            public int Score;
        }

        private void Start()
        {
            _director.Initialize();

            _elementTemplate.gameObject.SetActive(false);
            _elements = new Dictionary<CountryCode, DemoListElement>(8);

            _results.Sort((a, b) => b.Score - a.Score);
            for (int i = 0; i < _results.Count; i++)
            {
                var result = _results[i];
                var elem = RentElement();
                elem.Set(result.CountryCode, i + 1, result.Score);
            }
        }

        private DemoListElement RentElement()
        {
            var instance = Instantiate(_elementTemplate, _elementTemplate.transform.parent);
            instance.gameObject.SetActive(true);
            return instance;
        }

        [EditorPlayModeButton("Play")]
        public void Play()
        {
            _director.ParameterContainer.Vector3.Set("TargetPosition", _targetPosition);
            _director.Play();
        }
    }
}
