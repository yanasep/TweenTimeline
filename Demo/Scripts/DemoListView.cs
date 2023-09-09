using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TweenTimeline
{
    public class DemoListView : MonoBehaviour
    {
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private List<CountryResult> _countryResults;
        [SerializeField] private DemoListElement _elementTemplate;
        [SerializeField] private Button _randomAddButton;
        [SerializeField] private int scoreToAdd;
        
        private Dictionary<CountryCode, DemoListElement> _countryElements;
        
        [Serializable]
        private class CountryResult
        {
            public CountryCode CountryCode;
            public int Score;
        }

        private void Start()
        {
            _elementTemplate.gameObject.SetActive(false);
            _countryElements = new Dictionary<CountryCode, DemoListElement>(8);

            _countryResults.Sort((a, b) => b.Score - a.Score);
            for (int i = 0; i < _countryResults.Count; i++)
            {
                var result = _countryResults[i];
                var elem = RentElement();
                elem.Set(result.CountryCode, i + 1, result.Score);
                _countryElements.Add(result.CountryCode, elem);
            }
            
            _randomAddButton.onClick.AddListener(AddScoreRandom);
        }

        private DemoListElement RentElement()
        {
            var instance = Instantiate(_elementTemplate, _elementTemplate.transform.parent);
            instance.gameObject.SetActive(true);
            instance.Initialize();
            return instance;
        }
        
        private void AddScoreRandom()
        {
            int prevIndex = Random.Range(0, _countryResults.Count);
            var data = _countryResults[prevIndex];
            data.Score += scoreToAdd;
            var elem = _countryElements[data.CountryCode];
            elem.UpdateScore(data.Score);

            int newIndex = prevIndex;
            for (int i = 0; i < prevIndex; i++)
            {
                var other = _countryResults[i];
                if (data.Score > other.Score)
                {
                    newIndex = i;
                    break;
                }
            }
            
            if (newIndex == prevIndex) return;

            _countryResults.RemoveAt(prevIndex);
            _countryResults.Insert(newIndex, data);

            // 最初の子templateが入っている
            _countryElements[data.CountryCode].transform.SetSiblingIndex(newIndex + 1);

            for (int i = newIndex; i <= prevIndex; i++)
            {
                var d = _countryResults[i];
                var e = _countryElements[d.CountryCode];
                e.UpdatePlace(i + 1);
            }
        }
    }
}
