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
    public GameObject PlayerHandArea, EnemyHandArea;
    public GameObject CardSlot00, CardSlot01, CardSlot02, CardSlot10, CardSlot11, CardSlot12, CardSlot20, CardSlot21, CardSlot22, CardSlot30, CardSlot31, CardSlot32;
    public GameObject ZoomedCard, PlayerDeckSlot, EnemyDeckSlot;
    public GameObject CardPrefab;
    public Button EndTurnButton;
    public Button UndoButton;
    public List<Deck> DecksInGame = new List<Deck>();
    public List<Card> CardsInGame = new List<Card>();
    public List<CardObject> CardGameObjectsInGame = new List<CardObject>();
    public List<GameObject> StatBoxes = new List<GameObject>();
    private Dictionary<string, GameObject> SlotMap = new Dictionary<string, GameObject>();
    public TextMeshProUGUI TurnStateDisplay;
    public string EnemyLeader, PlayerLeader;

    public int MAX_CARDS_PER_DECK = 4;
    public int MIN_DECK_SIZE = 5;
    public int MAX_DECK_SIZE = 30;

    public GameObject SelectedCardGameObject;
    public State GameState = State.Moving;

    public static GameStart INSTANCE = null;

    public enum State
    {
        Moving,
        Summoning,
        Battle,
        EnemyTurn
    }

    public Card FindCard(string name)
    {
        Card card = null;
        foreach (Card c in CardsInGame)
        {
            if (c.imageName == name)
            {
                card = c;
                break;
            }
        }
        return card;
    }

    public CardObject FindCardObject(GameObject go)
    {
        CardObject co = null;
        foreach (CardObject c in CardGameObjectsInGame)
        {
            if (c.GameObject.GetInstanceID() == go.GetInstanceID())
            {
                co = c;
                break;
            }
        }
        return co;
        

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
        EnemyLeader = "Lambda";
        PlayerLeader = "Beatrice";
        Deck beatriceDeck = CreateDeck(PlayerLeader);
        Deck enemyDeck = CreateDeck(EnemyLeader);
        DecksInGame.Add(beatriceDeck);
        DecksInGame.Add(enemyDeck);
        AIFunctions.INSTANCE.deck = enemyDeck;
        CardGameObjectsInGame.Add(new CardObject(CreateCardInSlot(PlayerLeader, CardSlot31)));
        CardGameObjectsInGame.Add(new CardObject(CreateCardInSlot(EnemyLeader, CardSlot01)));
        Hand playerHand = new Hand();
        Hand enemyHand = new Hand();
        enemyHand.cards = DrawEnemyStartingHand(enemyDeck);
        AIFunctions.INSTANCE.hand = enemyHand;
        playerHand.cards = DrawPlayerStartingHand(beatriceDeck);
        InitializeSlotMap();
        RecalculateCosts();
        SaveState();
    }

    private void InitializeSlotMap()
    {
        SlotMap.Add("00", CardSlot00);
        SlotMap.Add("01", CardSlot01);
        SlotMap.Add("02", CardSlot02);
        SlotMap.Add("10", CardSlot10);
        SlotMap.Add("11", CardSlot11);
        SlotMap.Add("12", CardSlot12);
        SlotMap.Add("20", CardSlot20);
        SlotMap.Add("21", CardSlot21);
        SlotMap.Add("22", CardSlot22);
        SlotMap.Add("30", CardSlot30);
        SlotMap.Add("31", CardSlot31);
        SlotMap.Add("32", CardSlot32);
    }

    public void RecalculateCosts()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int cost = CheckCardsAround(i, j);
                GameObject slot = SlotMap[i.ToString() + j.ToString()];
                GameObject costBox = slot.transform.GetChild(2).gameObject;
                costBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cost.ToString();
            }
        }
    }

    private int CheckCardsAround(int i, int j)
    {
        int total = 0;
        total += CheckLeft(i, j);
        total += CheckRight(i, j);
        total += CheckUp(i, j);
        total += CheckDown(i, j);
        return total;
    }

    private int CheckDown(int i, int j)
    {
        if(i == 1 || i == 3)
        {
            return 0;
        }
        else
        {
            GameObject slot = SlotMap[(i+1).ToString() + j.ToString()];
            if (SlotWithCard(slot))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    private int CheckUp(int i, int j)
    {
        if (i == 0 || i == 2)
        {
            return 0;
        }
        else
        {
            GameObject slot = SlotMap[(i-1).ToString() + j.ToString()];
            if (SlotWithCard(slot))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    private int CheckRight(int i, int j)
    {
        if (j == 2)
        {
            return 0;
        }
        else
        {
            GameObject slot = SlotMap[i.ToString() + (j+1).ToString()];
            if (SlotWithCard(slot))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    private int CheckLeft(int i, int j)
    {
        if (j == 0)
        {
            return 0;
        }
        else
        {
            GameObject slot = SlotMap[i.ToString() + (j - 1).ToString()];
            if (SlotWithCard(slot))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        TurnStateDisplay.text = GameState.ToString();
    }

    //Returns true if the attack destroys the card
    public bool Attack(GameObject defenderSlot, int damage)
    {
        GameObject hpbox = defenderSlot.transform.GetChild(0).GetChild(0).gameObject;
        TextMeshProUGUI hpText = hpbox.GetComponent<TextMeshProUGUI>();
        int newHP = (Int32.Parse(hpText.text) - damage);
        //if attacking leader
        if(newHP <= 0)
        {
            GameObject atkbox = defenderSlot.transform.GetChild(1).GetChild(0).gameObject;
            TextMeshProUGUI atkText = atkbox.GetComponent<TextMeshProUGUI>();
            atkText.text = 0.ToString();
            hpText.text = 0.ToString();
            if (EnemyLeader == defenderSlot.GetComponent<Image>().sprite.name)
            {
                EnemyLeaderImage.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = 0.ToString();
            }
            return true;
        }
        else
        {
            hpText.text = newHP.ToString(); 
            if (EnemyLeader == defenderSlot.transform.GetChild(3).GetComponent<Image>().sprite.name)
            {
                EnemyLeaderImage.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = newHP.ToString();
            }
            return false;
        }
    }

    internal void Victory()
    {
        Debug.Log("GG");
    }

    private void OnEndTurn()
    {
        if (GameState != State.EnemyTurn)
        {
            SaveState();
            GameState = State.EnemyTurn;
        }
    }

    private void SaveState()
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
        foreach (CardObject go in CardGameObjectsInGame)
        {
            SaveGameObject(go.GameObject);
        }
        PlayerPrefs.SetString("PlayerLifePoints", PlayerLifePoints.GetComponent<TextMeshProUGUI>().text);
        PlayerPrefs.SetString("EnemyLifePoints", EnemyLifePoints.GetComponent<TextMeshProUGUI>().text);
        AIFunctions.INSTANCE.TakeTurn();
    }

    public void OnTurnStart()
    {
        GameState = State.Moving;
        RestoreCards();
    }

    private void RestoreCards()
    {
        foreach(CardObject co in CardGameObjectsInGame)
        {
            GameObject go = co.GameObject;
            Image goImage = go.GetComponent<Image>();
            goImage.color = new Color32(255, 255, 255, 255);
            Card card = FindCard(goImage.sprite.name);
            co.acted = false;
            co.usedEffect = false;
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
            foreach (CardObject go in CardGameObjectsInGame)
            {
                LoadGameObject(go.GameObject);
            }
            PlayerLifePoints.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("PlayerLifePoints");
            EnemyLifePoints.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("EnemyLifePoints");
            GameState = State.Summoning;
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
        UpdateStatBoxes(card, cardSlot:cardSlot);
        
        return go;
    }

    public void UpdateStatBoxes(Card card, GameObject cardSlot, GameObject previousParent = null)
    {
        string slotNumber = cardSlot.name.Substring(8, 2);
        GameObject hpGODad = cardSlot.transform.Find("HPBlock" + slotNumber).gameObject;
        GameObject hpGO = hpGODad.transform.GetChild(0).gameObject;
        GameObject atkGODad = cardSlot.transform.Find("ATKBlock" + slotNumber).gameObject;
        GameObject atkGO = atkGODad.transform.GetChild(0).gameObject;
        GameObject costGODad = cardSlot.transform.Find("CostBlock" + slotNumber).gameObject;
        GameObject costGO = costGODad.transform.GetChild(0).gameObject;
        hpGO.GetComponent<TextMeshProUGUI>().text = card.hp.ToString();
        atkGO.GetComponent<TextMeshProUGUI>().text = card.attack.ToString();
        //costGO.GetComponent<TextMeshProUGUI>().text = card.cost.ToString();
        hpGODad.SetActive(true);
        atkGODad.SetActive(true);
        //costGODad.SetActive(true);

        //If it was in another slot and not in the hand
        if(previousParent != null && previousParent.name != "PlayerHandArea")
        {
            string ppslotNumber = previousParent.name.Substring(8, 2);
            GameObject pphpGODad = previousParent.transform.Find("HPBlock" + ppslotNumber).gameObject;
            GameObject ppatkGODad = previousParent.transform.Find("ATKBlock" + ppslotNumber).gameObject;
            GameObject ppcostGODad = previousParent.transform.Find("CostBlock" + ppslotNumber).gameObject;
            pphpGODad.SetActive(false);
            ppatkGODad.SetActive(false);
            //ppcostGODad.SetActive(false);
        }

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
        if(go != null)
        {
            GameObject savedObjectParent = GameObject.Find(PlayerPrefs.GetString(go.name));
            go.transform.SetParent(savedObjectParent.gameObject.transform, false);
            float x = PlayerPrefs.GetFloat(go.name + "x");
            float y = PlayerPrefs.GetFloat(go.name + "y");
            go.GetComponent<RectTransform>().localPosition = new Vector2(x, y);
        }
    }

    private bool SlotWithCard(GameObject go)
    {
        return go.transform.childCount > 3;
    }

    public void RearrangeHand(Hand hand, bool playerHand)
    {

    }

}
