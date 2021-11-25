using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public GameObject PlayerLeaderImage, EnemyLeaderImage;
    public GameObject PlayerHandCard1, PlayerHandCard2, PlayerHandCard3, PlayerHandCard4, PlayerHandCard5;
    public GameObject EnemyHandCard1, EnemyHandCard2, EnemyHandCard3, EnemyHandCard4, EnemyHandCard5;
    public GameObject CardSlot00, CardSlot01, CardSlot02, CardSlot10, CardSlot11, CardSlot12, CardSlot20, CardSlot21, CardSlot22, CardSlot30, CardSlot31, CardSlot32;
    public GameObject ZoomedCard, PlayerDeckSlot, EnemyDeckSlot;
    public GameObject CardPrefab;
    public List<Deck> DecksInGame = new List<Deck>();
    public int[][] field;

    public int MAX_CARDS_PER_DECK = 4;
    public int MIN_DECK_SIZE = 5;
    public int MAX_DECK_SIZE = 30;

    public GameObject selectedCardGameObject;

    public static GameStart INSTANCE = null;


    // Start is called before the first frame update
    void Start()
    {
        if(INSTANCE == null)
        {
            INSTANCE = this;
        }
        Debug.Log("Starting");
        CreateCardInSlot("Beatrice", CardSlot11);
        CreateCardInSlot("Lambda", CardSlot21);
        Deck beatriceDeck = CreateDeck("Beatrice");
        Deck lambdaDeck = CreateDeck("Lambda");
        DecksInGame.Add(beatriceDeck);
        DecksInGame.Add(lambdaDeck);
        Hand playerHand = new Hand();
        Hand enemyHand = new Hand();
        enemyHand.cards = DrawEnemyStartingHand(beatriceDeck);
        playerHand.cards = DrawPlayerStartingHand(lambdaDeck);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void CreateCardInSlot(string cardName, GameObject cardSlot)
    {
        Sprite enemyLeaderSprite = (Sprite)Resources.Load("cards/" + cardName, typeof(Sprite));
        GameObject go = new GameObject(cardName + "Card");
        Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        RectTransform rectTransform = go.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(112, 160);
        Image image = go.AddComponent<Image>();
        image.sprite = enemyLeaderSprite;
        go.transform.SetParent(cardSlot.transform, false);
        CardZoom script = go.AddComponent<CardZoom>();
        script.ZoomedCard = ZoomedCard;
    }

    private Deck CreateDeck(string leader)
    {
        Deck deck = new Deck();
        deck.InitializeDeck(leader);
        deck.Shuffle();
        return deck;
    }

    


    public Card DrawOne(Deck deck)
    {
        Card drawnCard = deck.cards.Pop();
        return drawnCard;
    }

    public List<Card> DrawEnemyStartingHand(Deck startingdeck)
    {
        List<Card> ret = new List<Card>();
        Card drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        Sprite sprite = GetCardSprite("cardback");
        EnemyHandCard1.GetComponent<Image>().sprite = sprite;
        EnemyHandCard1.SetActive(true);
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        EnemyHandCard2.GetComponent<Image>().sprite = sprite;
        EnemyHandCard2.SetActive(true);
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        EnemyHandCard3.GetComponent<Image>().sprite = sprite;
        EnemyHandCard3.SetActive(true);
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        EnemyHandCard4.GetComponent<Image>().sprite = sprite;
        EnemyHandCard4.SetActive(true);
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        EnemyHandCard5.GetComponent<Image>().sprite = sprite;
        EnemyHandCard5.SetActive(true);
        return ret;
    }

    private Sprite GetCardSprite(string name)
    {
        return (Sprite)Resources.Load("cards/" + name, typeof(Sprite));
    }

    public List<Card> DrawPlayerStartingHand(Deck startingdeck)
    {
        List<Card> ret = new List<Card>();
        Card drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        Sprite sprite1 = (Sprite)Resources.Load("cards/" + drawnCard.imageName, typeof(Sprite));
        PlayerHandCard1.GetComponent<Image>().sprite = sprite1;
        PlayerHandCard1.SetActive(true);
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        Sprite sprite2 = (Sprite)Resources.Load("cards/" + drawnCard.imageName, typeof(Sprite));
        PlayerHandCard2.GetComponent<Image>().sprite = sprite2;
        PlayerHandCard2.SetActive(true);
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        Sprite sprite3 = (Sprite)Resources.Load("cards/" + drawnCard.imageName, typeof(Sprite));
        PlayerHandCard3.GetComponent<Image>().sprite = sprite3;
        PlayerHandCard3.SetActive(true);
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        Sprite sprite4 = (Sprite)Resources.Load("cards/" + drawnCard.imageName, typeof(Sprite));
        PlayerHandCard4.GetComponent<Image>().sprite = sprite4;
        PlayerHandCard4.SetActive(true);
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        Sprite sprite5 = (Sprite)Resources.Load("cards/" + drawnCard.imageName, typeof(Sprite));
        PlayerHandCard5.GetComponent<Image>().sprite = sprite5;
        PlayerHandCard5.SetActive(true);
        return ret;
    }
}
