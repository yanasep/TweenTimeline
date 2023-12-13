using System.Collections.Generic;
using System.Threading;
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
        private CancellationTokenSource cts;

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
            cts?.Cancel();
            cts = new CancellationTokenSource();
            
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

            await director.Play(param =>
            {
                param.SetVector3("DropPosition", dropScreenPos);
            }).ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken: cts.Token);

            for (int i = 0; i < cardParents.Length; i++)
            {
                var trackName = string.Format(cardTrackNameFormat, i);
                director.SetTrackBinding(director.PlayableAsset, trackName, null);
            }
        }
    }
}