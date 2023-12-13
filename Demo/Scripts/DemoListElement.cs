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

        public void Initialize()
        {
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
            return _tweenDirector.Play(_scoreUpTween, parameter =>
            {
                parameter.SetInt("Score", score);
            }).ToUniTask();
        }

        public UniTask UpdatePlaceAsync(int place)
        {
            placeText.SetText("{0}", place);
            return _tweenDirector.Play(_changePlaceTween).ToUniTask();
        }
    }
}