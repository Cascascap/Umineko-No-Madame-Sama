using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public GameObject PlayerLeaderImage, EnemyLeaderImage, PlayerLifePoints, EnemyLifePoints;
    public GameObject PlayerHandCard1, PlayerHandCard2, PlayerHandCard3, PlayerHandCard4, PlayerHandCard5;
    public GameObject EnemyHandCard1, EnemyHandCard2, EnemyHandCard3, EnemyHandCard4, EnemyHandCard5;
    public GameObject CardSlot00, CardSlot01, CardSlot02, CardSlot10, CardSlot11, CardSlot12, CardSlot20, CardSlot21, CardSlot22, CardSlot30, CardSlot31, CardSlot32;
    public GameObject ZoomedCard, PlayerDeckSlot, EnemyDeckSlot;
    public GameObject CardPrefab;
    public Button EndTurnButton;
    public Button UndoButton;
    public List<Deck> DecksInGame = new List<Deck>();
    public List<GameObject> CardsInGame = new List<GameObject>();
    public List<GameObject> StatBoxes = new List<GameObject>();
    public int[][] field;

    public int MAX_CARDS_PER_DECK = 4;
    public int MIN_DECK_SIZE = 5;
    public int MAX_DECK_SIZE = 30;

    public GameObject SelectedCardGameObject;
    public State GameState = State.Summoning;

    public static GameStart INSTANCE = null;

    public enum State
    {
        Summoning,
        Moving,
        Battle,
        EnemyTurn
    }

    public Card FindCard(string name)
    {
        Card card = null;
        foreach (Deck d in GameStart.INSTANCE.DecksInGame)
        {
            Card c = d.FindCardInDeck(name);
            if (c != null)
            {
                card = c;
                break;
            }
        }
        return card;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(INSTANCE == null)
        {
            INSTANCE = this;
        }
        Debug.Log("Starting"); 
        Button EndTurnbtn = EndTurnButton.GetComponent<Button>();
        Button UndoBtn = UndoButton.GetComponent<Button>();
        EndTurnbtn.onClick.AddListener(OnEndTurn);
        UndoBtn.onClick.AddListener(Undo);
        Deck beatriceDeck = CreateDeck("Beatrice");
        Deck lambdaDeck = CreateDeck("Lambda");
        DecksInGame.Add(beatriceDeck);
        DecksInGame.Add(lambdaDeck);
        CardsInGame.Add(CreateCardInSlot("Beatrice", CardSlot21));
        CardsInGame.Add(CreateCardInSlot("Lambda", CardSlot11));
        Hand playerHand = new Hand();
        Hand enemyHand = new Hand();
        enemyHand.cards = DrawEnemyStartingHand(lambdaDeck);
        playerHand.cards = DrawPlayerStartingHand(beatriceDeck);
        OnEndTurn();
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void OnEndTurn()
    {
        if (GameState != State.EnemyTurn)
        {
            foreach (GameObject go in StatBoxes)
            {
                SaveStatBox(go);
            }
            List<GameObject> cardsInHand = new List<GameObject> { PlayerHandCard1, PlayerHandCard2, PlayerHandCard3, PlayerHandCard4, PlayerHandCard5 };
            foreach (GameObject go in cardsInHand)
            {
                SaveGameObject(go);
            }
            foreach (GameObject go in CardsInGame)
            {
                SaveGameObject(go);
            }
            PlayerPrefs.SetString("PlayerLifePoints", PlayerLifePoints.GetComponent<TextMeshProUGUI>().text);
            PlayerPrefs.SetString("EnemyLifePoints", EnemyLifePoints.GetComponent<TextMeshProUGUI>().text);
        }
        
    }

    private void Undo()
    {
        if(GameState != State.EnemyTurn)
        {
            foreach (GameObject go in StatBoxes)
            {
                LoadStatBox(go.name);
            }
            List<GameObject> cardsInHand = new List<GameObject> { PlayerHandCard1, PlayerHandCard2, PlayerHandCard3, PlayerHandCard4, PlayerHandCard5 };
            foreach (GameObject go in cardsInHand)
            {
                LoadGameObject(go);
            }
            foreach (GameObject go in CardsInGame)
            {
                LoadGameObject(go);
            }
            PlayerLifePoints.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("PlayerLifePoints");
            EnemyLifePoints.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("EnemyLifePoints");
        }
    }

    public GameObject CreateCardInSlot(string cardName, GameObject cardSlot)
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

        Card card = FindCard(cardName);
        string slotNumber = cardSlot.name.Substring(8, 2);
        GameObject hpGODad = cardSlot.transform.Find("HPBlock"+slotNumber).gameObject;
        GameObject hpGO = hpGODad.transform.GetChild(0).gameObject;
        GameObject atkGODad = cardSlot.transform.Find("ATKBlock" + slotNumber).gameObject;
        GameObject atkGO = atkGODad.transform.GetChild(0).gameObject;
        GameObject costGODad = cardSlot.transform.Find("CostBlock" + slotNumber).gameObject;
        GameObject costGO = costGODad.transform.GetChild(0).gameObject;
        hpGO.GetComponent<TextMeshProUGUI>().text = card.hp.ToString();
        atkGO.GetComponent<TextMeshProUGUI>().text = card.attack.ToString();
        costGO.GetComponent<TextMeshProUGUI>().text = card.cost.ToString();
        hpGODad.SetActive(true);
        atkGODad.SetActive(true);
        costGODad.SetActive(true);
        return go;
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

    private void SaveStatBox(GameObject go)
    {
        GameObject boxText = go.transform.GetChild(0).gameObject;
        TextMeshProUGUI text = boxText.GetComponent<TextMeshProUGUI>();
        PlayerPrefs.SetString(go.name, text.text);
    }

    private void LoadStatBox(string lifeBoxName)
    {
        GameObject go = GameObject.Find(lifeBoxName);
        if(go != null)
        {
            go = go.transform.GetChild(0).gameObject;
            TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
            text.text = PlayerPrefs.GetString(lifeBoxName);
        }
    }
    private void SaveGameObject(GameObject go)
    {
        PlayerPrefs.SetString(go.name, go.transform.parent.name);
        PlayerPrefs.SetFloat(go.name + "x", go.transform.localPosition.x);
        PlayerPrefs.SetFloat(go.name + "y", go.transform.localPosition.y);
    }

    private void LoadGameObject(GameObject go)
    {
        GameObject savedObjectParent = GameObject.Find(PlayerPrefs.GetString(go.name));
        go.transform.SetParent(savedObjectParent.gameObject.transform, false);
        float x = PlayerPrefs.GetFloat(go.name + "x");
        float y = PlayerPrefs.GetFloat(go.name + "y");
        go.GetComponent<RectTransform>().localPosition = new Vector2(x, y);
    }

}
