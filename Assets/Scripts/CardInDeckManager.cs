using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInDeckManager : MonoBehaviour
{
    [SerializeField] private int Height;
    [SerializeField] private CardInDeck CardInDeckPrefab;
    [SerializeField] private GameObject Background;
    [SerializeField] public Image ZoomedCard;

    private List<Card> CardsInDeck;
    private int INITIALY = -25;
    private int OFFSET = -5;

    void Start()
    {
        Deck deck = new Deck();
        deck.LoadDeck();
        CardsInDeck = new List<Card>(deck.cards);
        for (int i=0; i< CardsInDeck.Count; i++)
        {
            CardInDeck spawnedCard = Instantiate(CardInDeckPrefab, new Vector3(0, 0), Quaternion.identity, Background.transform);
            RectTransform rectTransform = spawnedCard.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector2(3, INITIALY + OFFSET - (Height*i));
            spawnedCard.name = "CardInDeck" + i.ToString();
            Card cardInDeck = CardsInDeck[i];
            spawnedCard.CardInDeckName.text = cardInDeck.ImageName;
            spawnedCard.CardInDeckCost.text = cardInDeck.Cost.ToString();
            spawnedCard.CardInDeckHP.text = cardInDeck.HP.ToString();
            spawnedCard.CardInDeckATK.text = cardInDeck.Attack.ToString();
            Sprite sprite = (Sprite)Resources.Load("cards/" + cardInDeck.ImageName, typeof(Sprite));
            spawnedCard.CardInDeckSprite = sprite;
            spawnedCard.ZoomedCard = ZoomedCard;
        }

    }
}
