using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Yanasep;

namespace TweenTimeline.Demo2
{
    public class CardsTweenDemo : MonoBehaviour
    {
        public string cardTrackNameFormat = "Card{0}";
        public Card cardPrefab;
        public Transform[] cardParents;
        public TweenTimelineDirector director;
        public Transform dropSourceUIObj;
        private readonly List<Card> cards = new();

        private static readonly int Vec3ParamDropPosition = TweenParameter.StringToHash("DropPosition");

        private void Start()
        {
            foreach (var cardParent in cardParents)
            {
                foreach (Transform child in cardParent)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        [EditorPlayModeButton("Play")]
        public async UniTask Play()
        {
            for (int i = cards.Count; i < cardParents.Length; i++)
            {
                var card = Instantiate(cardPrefab, cardParents[i]);
                var color = Color.HSVToRGB((float)i / cardParents.Length, 1, 1);
                card.Set($"カード\n{i}", color);
                cards.Add(card);
            }

            for (int i = 0; i < cardParents.Length; i++)
            {
                director.SetTrackBinding(director.PlayableAsset, string.Format(cardTrackNameFormat, i), cards[i].director);
            }

            var dropScreenPos = dropSourceUIObj.position;

            await director.PlayAsync(param =>
            {
                param.Vector3.Set(Vec3ParamDropPosition, dropScreenPos);
            });

            for (int i = 0; i < cardParents.Length; i++)
            {
                var trackName = string.Format(cardTrackNameFormat, i);
                director.SetTrackBinding(director.PlayableAsset, trackName, null);
            }
        }
    }
}