using Cysharp.Threading.Tasks;
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
            placeText.SetText("{0}", place);
            scoreText.SetText("{0}", score);
        }

        public UniTask UpdateScoreAsync(int score)
        {   
            _tweenDirector.Parameter.Int.Set(TweenHashScore, score);
            return _tweenDirector.PlayAsync(_scoreUpTween);
        }

        public UniTask UpdatePlaceAsync(int place)
        {
            placeText.SetText("{0}", place);
            return _tweenDirector.PlayAsync(_changePlaceTween);
        }
    }
}