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
    public GameObject[] Cards;
    public int[][] field;

    public int MAX_CARDS_PER_DECK = 4;
    public int MAX_DECK_SIZE = 30;

    public static Dictionary<string, Card> cardList = Card.getAllCards();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting");
        PreparePlayerLeader("Beatrice");
        PrepareEnemyLeader("Lambda");
        Deck beatriceDeck = CreateDeck("Beatrice");
        Hand playerHand = new Hand();
        playerHand.cards = DrawStartingHand(beatriceDeck);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PrepareEnemyLeader(string leader)
    {
        Sprite BeatriceSprite = (Sprite)Resources.Load("cards/" + leader, typeof(Sprite));
        Image twentyOneImage = CardSlot11.transform.Find("Image").GetComponent<Image>();
        twentyOneImage.sprite = BeatriceSprite;
        (twentyOneImage.gameObject).SetActive(true);
    }

    private void PreparePlayerLeader(string leader)
    {
        Sprite BeatriceSprite = (Sprite)Resources.Load("cards/" + leader, typeof(Sprite));
        Image twentyOneImage = CardSlot21.transform.Find("Image").GetComponent<Image>();
        twentyOneImage.sprite = BeatriceSprite;
        (twentyOneImage.gameObject).SetActive(true);
    }

    private Deck CreateDeck(string leader)
    {
        Deck beatriceDeck = new Deck();
        beatriceDeck.InitializeDeck(leader);
        beatriceDeck.Shuffle();
        return beatriceDeck;
    }

    


    public Card DrawOne(Deck deck)
    {
        Card drawnCard = deck.cards.Pop();
        return drawnCard;
    }

    public List<Card> DrawStartingHand(Deck startingdeck)
    {
        List<Card> ret = new List<Card>();
        Card drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        Sprite sprite1 = (Sprite)Resources.Load("cards/" + drawnCard.imageName, typeof(Sprite));
        PlayerHandCard1.GetComponent<Image>().sprite = sprite1;
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        Sprite sprite2 = (Sprite)Resources.Load("cards/" + drawnCard.imageName, typeof(Sprite));
        PlayerHandCard2.GetComponent<Image>().sprite = sprite2;
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        Sprite sprite3 = (Sprite)Resources.Load("cards/" + drawnCard.imageName, typeof(Sprite));
        PlayerHandCard3.GetComponent<Image>().sprite = sprite3;
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        Sprite sprite4 = (Sprite)Resources.Load("cards/" + drawnCard.imageName, typeof(Sprite));
        PlayerHandCard4.GetComponent<Image>().sprite = sprite4;
        drawnCard = startingdeck.cards.Pop();
        ret.Add(drawnCard);
        Sprite sprite5 = (Sprite)Resources.Load("cards/" + drawnCard.imageName, typeof(Sprite));
        PlayerHandCard5.GetComponent<Image>().sprite = sprite5;
        return ret;
    }
}
