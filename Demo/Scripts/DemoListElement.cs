using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TweenTimeline
{
    public class DemoListElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI placeText;
        [SerializeField] private TextMeshProUGUI countryNameText;
        [SerializeField] private Image countryFlagImage;
        [SerializeField] private TextMeshProUGUI scoreText;

        public void Set(CountryCode country, int place, int score)
        {
            var data = CountryDB.Instance.Get(country);
            countryNameText.SetText(data.CountryName);
            countryFlagImage.sprite = data.CountryFlag;
            UpdatePlace(place);
            UpdateScore(score);
        }

        public void UpdateScore(int score)
        {
            scoreText.SetText("{0}", score);
        }

        public void UpdatePlace(int place)
        {
            placeText.SetText("{0}", place);
        }
    }
}