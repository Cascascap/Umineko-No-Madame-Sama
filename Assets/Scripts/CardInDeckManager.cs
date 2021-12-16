using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardInDeckManager : MonoBehaviour
{
    [SerializeField] private int Height;
    [SerializeField] private CardInDeck CardInDeckPrefab;
    [SerializeField] private GameObject Background;
    [SerializeField] public Image ZoomedCard;
    [SerializeField] private TextMeshProUGUI CardsInDeckText;
    [SerializeField] private DeckGrid DeckGrid;

    private static CardInDeckManager INSTANCE;
    private Deck Deck;
    public List<Card> CardsInDeck;
    private int INITIALY = -25;
    private int OFFSET = -5;

    public int MAX_CARDS_PER_DECK = 4;
    public int MIN_DECK_SIZE = 5;
    public int MAX_DECK_SIZE = 30;

    public static CardInDeckManager GetInstance()
    {
        return INSTANCE;
    }
    void Start()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            Deck = new Deck();
            Deck.LoadDeck();
            CardsInDeck = new List<Card>(Deck.cards);
        }
        PrintDeck();
    }

    private void PrintDeck()
    {
        CardsInDeckText.text = $"{CardsInDeck.Count}/{MAX_DECK_SIZE}";
        if (Background.transform.childCount > 1)
        {
            for(int i=1; i< Background.transform.childCount; i++)
            {
                GameObject child = Background.transform.GetChild(i).gameObject;
                GameObject.Destroy(child);
            }
        }
        for (int i = 0; i < CardsInDeck.Count; i++)
        {
            CardInDeck spawnedCard = Instantiate(CardInDeckPrefab, new Vector3(0, 0), Quaternion.identity, Background.transform);
            RectTransform rectTransform = spawnedCard.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector2(0, INITIALY + OFFSET - (Height * i));
            spawnedCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
            spawnedCard.name = "CardInDeck" + i.ToString();
            Card cardInDeck = CardsInDeck[i];
            spawnedCard.CardInDeckName.text = cardInDeck.ImageName;
            spawnedCard.CardInDeckCost.text = cardInDeck.Cost.ToString();
            spawnedCard.CardInDeckHP.text = cardInDeck.HP.ToString();
            spawnedCard.CardInDeckATK.text = cardInDeck.Attack.ToString();
            Sprite sprite = (Sprite)Resources.Load("cards/" + cardInDeck.ImageName, typeof(Sprite));
            spawnedCard.CardInDeckSprite = sprite;
            spawnedCard.ZoomedCard = ZoomedCard;
            spawnedCard.Card = cardInDeck;
        }
    }

    public void AddCardToDeck(DeckGridTile card)
    {
        if(PlayerPrefs.GetInt(card.Card.ImageName + "Inventory") == 0)
        {
            return;
        }
        int cardsInDeck = PlayerPrefs.GetInt(card.Card.ImageName);
        PlayerPrefs.SetInt(card.Card.ImageName, cardsInDeck + 1);
        int cardsInInventory = PlayerPrefs.GetInt(card.Card.ImageName + "Inventory");
        PlayerPrefs.SetInt(card.Card.ImageName + "Inventory", cardsInInventory - 1);
        if(cardsInInventory == 0)
        {
            card.gameObject.SetActive(false);
        }
        CardsInDeck.Add(card.Card);
        DeckGrid.LoadCardsInInventory();
        PrintDeck();
    }

    public void RemoveCardFromDeck(CardInDeck card)
    {
        if(CardsInDeck.Count <= MIN_DECK_SIZE)
        {
            EditorUtility.DisplayDialog("Can't do action", $"Cant have less than {MIN_DECK_SIZE} cards in deck", "OK");
            Debug.Log($"Cant have less than {MIN_DECK_SIZE} cards in deck");
            return;
        }
        int cardsInDeck = PlayerPrefs.GetInt(card.Card.ImageName);
        PlayerPrefs.SetInt(card.Card.ImageName, cardsInDeck - 1);
        int cardsInInventory = PlayerPrefs.GetInt(card.Card.ImageName+"Inventory");
        PlayerPrefs.SetInt(card.Card.ImageName + "Inventory", cardsInInventory + 1);
        CardsInDeck.Remove(card.Card);
        DeckGrid.LoadCardsInInventory();
        PrintDeck();
    }
}
