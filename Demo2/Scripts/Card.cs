using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace TweenTimeline.Demo2
{
    public class Card : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public Image colorImage;
        public PlayableDirector director;

        public void Set(string label, Color color)
        {
            text.text = label;
            colorImage.color = color;
        }
    }
}