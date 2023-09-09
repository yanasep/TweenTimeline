using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace TweenTimeline
{
    public class DemoListElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI placeText;
        [SerializeField] private TextMeshProUGUI countryNameText;
        [SerializeField] private Image countryFlagImage;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TweenTimelineDirector _tweenDirector;
        [SerializeField] private TimelineAsset _scoreUpTween;
        [SerializeField] private TimelineAsset _changePlaceTween;

        private static readonly int TweenHashScore = TweenParameter.StringToHash("Score");

        public void Initialize()
        {
            _tweenDirector.Initialize();
        }

        public void Set(CountryCode country, int place, int score)
        {
            var data = CountryDB.Instance.Get(country);
            countryNameText.SetText(data.CountryName);
            countryFlagImage.sprite = data.CountryFlag;
            UpdatePlace(place);
            UpdateScore(score, false);
        }

        public void UpdateScore(int score, bool playAnimation = true)
        {   
            if (playAnimation)
            {
                _tweenDirector.Parameter.Int.Set(TweenHashScore, score);
                _tweenDirector.Play(_scoreUpTween);
            }
            else
            {
                scoreText.SetText("{0}", score);
            }
        }

        public void UpdatePlace(int place, bool playAnimation = true)
        {
            placeText.SetText("{0}", place);
        }
    }
}