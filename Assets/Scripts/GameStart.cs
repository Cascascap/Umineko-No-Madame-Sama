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
    public GameObject PlayerHandArea, EnemyHandArea;
    public GameObject CardSlot00, CardSlot01, CardSlot02, CardSlot10, CardSlot11, CardSlot12, CardSlot20, CardSlot21, CardSlot22, CardSlot30, CardSlot31, CardSlot32;
    public GameObject ZoomedCard, PlayerDeckSlot, EnemyDeckSlot, PlayerGraveyard, EnemyGraveyard, PlayerField, EnemyField;
    public GameObject CardPrefab;
    public Button EndTurnButton;
    public Button UndoButton;
    public List<Deck> DecksInGame = new List<Deck>();
    public List<Card> CardsInGame = new List<Card>();
    public List<CardObject> CardGameObjectsInGame = new List<CardObject>();
    public List<GameObject> StatBoxes = new List<GameObject>();
    public GameObject CountersPrefab;


    private Dictionary<string, GameObject> SlotMap = new Dictionary<string, GameObject>();
    public TextMeshProUGUI TurnStateDisplay;

    public int MAX_CARDS_PER_DECK = 4;
    public int MIN_DECK_SIZE = 5;
    public int MAX_DECK_SIZE = 30;
    public int MAX_CARDS_HAND = 8;
    public int STARTING_CARDS_HAND = 5;

    public GameObject SelectedCardGameObject;
    public State GameState = State.Moving;
    public int Turn = 0;

    public static GameStart INSTANCE = null;

    //TODO: move to a leader class
    public string EnemyLeader, PlayerLeader;
    public Deck PlayerDeck, EnemyDeck;
    public Hand PlayerHand, EnemyHand;
    public Card CardUsingEffect;

    public Dictionary<string, GameObject> GetSlotMap()
    {
        return this.SlotMap;
    }

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
            if (c.ImageName == name)
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

    public CardObject FindEnemyLeaderCardObject(string leaderName)
    {
        CardObject co = null;
        foreach (CardObject c in CardGameObjectsInGame)
        {
            if (c.GameObject.name == leaderName + "Card")
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
        PlayerDeck = CreateDeck(PlayerLeader);
        EnemyDeck = CreateDeck(EnemyLeader);
        DecksInGame.Add(PlayerDeck);
        DecksInGame.Add(EnemyDeck);
        CardObject leaderCardObject = CreateCardInSlot(PlayerLeader, CardSlot21);
        CardGameObjectsInGame.Add(leaderCardObject);
        CardGameObjectsInGame.Add(CreateCardInSlot(EnemyLeader, CardSlot11));
        PlayerHand = new Hand();
        EnemyHand = new Hand();
        EnemyHand.cards = Draw(EnemyDeck, STARTING_CARDS_HAND);
        RearrangeHand(false);
        PlayerHand.cards = Draw(PlayerDeck, STARTING_CARDS_HAND);
        RearrangeHand(true);
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

    public int CheckDown(int i, int j)
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

    public int CheckUp(int i, int j)
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

    public int CheckRight(int i, int j)
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

    public int CheckLeft(int i, int j)
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
        foreach(CardObject co in CardGameObjectsInGame)
        {
            if (co.counters == 0 && co.GameObject.transform.childCount > 0)
            {
                for (int i = 0; i < co.GameObject.transform.childCount; i++)
                {
                    GameObject child = co.GameObject.transform.GetChild(i).gameObject;
                    if (child.name.Contains("CounterPanel"))
                    {
                        GameObject.Destroy(child);
                    }
                }
            }
        }
    }

    //Returns true if the attack destroys the card
    public bool Attack(GameObject defenderSlot, int damage)
    {
        GameObject cardObject = defenderSlot.transform.GetChild(3).gameObject;
        CardObject co = GameStart.INSTANCE.FindCardObject(cardObject);
        GameObject hpbox = defenderSlot.transform.GetChild(0).GetChild(0).gameObject;
        TextMeshProUGUI hpText = hpbox.GetComponent<TextMeshProUGUI>();
        int newHP = (Int32.Parse(hpText.text) - damage);
        //if attacking leader
        if(newHP <= 0)
        {
            GameObject atkbox = defenderSlot.transform.GetChild(1).GetChild(0).gameObject;
            hpText.text = 0.ToString();
            co.currentHP = 0;
            if (EnemyLeader == cardObject.GetComponent<Image>().sprite.name)
            {
                EnemyLeaderImage.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = 0.ToString();
            }
            return true;
        }
        else
        {
            hpText.text = newHP.ToString();
            co.currentHP = newHP;
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
            GameState = State.EnemyTurn;
            AIFunctions.INSTANCE.TakeTurn(EnemyDeck);
            Turn++;
        }
    }


    public void OnTurnStart()
    {
        GameState = State.Moving;
        if (PlayerHand.cards.Count < MAX_CARDS_HAND)
        {
            Card drawnCard = Draw(PlayerDeck, 1)[0];
            PlayerHand.cards.Add(drawnCard);
        }
        RestoreCards();
        RearrangeHand(true);
        SaveState();
    }


    private void SaveState()
    {
        foreach (GameObject go in StatBoxes)
        {
            SaveStatBox(go);
        }
        for(int i=0; i< PlayerHandArea.transform.childCount; i++)
        {
            GameObject go = PlayerHandArea.transform.GetChild(i).gameObject;
            SaveGameObject(go);
        }
        foreach (CardObject co in CardGameObjectsInGame)
        {
            SaveCardObject(co);
        }
        PlayerPrefs.SetString("PlayerLifePoints", PlayerLifePoints.GetComponent<TextMeshProUGUI>().text);
        PlayerPrefs.SetString("EnemyLifePoints", EnemyLifePoints.GetComponent<TextMeshProUGUI>().text);
    }

    private void Undo()
    {
        if (GameState != State.EnemyTurn)
        {
            foreach (GameObject go in StatBoxes)
            {
                LoadStatBox(go);
            }
            for (int i = 0; i < PlayerHandArea.transform.childCount; i++)
            {
                GameObject go = PlayerHandArea.transform.GetChild(i).gameObject;
                LoadGameObject(go);
            }
            foreach (CardObject go in CardGameObjectsInGame)
            {
                LoadCardObject(go);
            }
            PlayerLifePoints.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("PlayerLifePoints");
            EnemyLifePoints.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("EnemyLifePoints");
            GameState = State.Moving;
            CardUsingEffect = null;
            CardZoom.RemovePreviousMark();
        }
    }

    public void UseCardEffect(CardObject co, GameObject objective)
    {
        Debug.Log("Using " + co.card.ImageName + "'s effect");
        co.usedEffect = true;
        co.TurnEffectWasUsedOn = GameStart.INSTANCE.Turn;
        co.card.Effect(objective);
    }

    private void SaveStatBox(GameObject go)
    {
        GameObject boxText = go.transform.GetChild(0).gameObject;
        TextMeshProUGUI text = boxText.GetComponent<TextMeshProUGUI>();
        PlayerPrefs.SetString(go.name, text.text);
        PlayerPrefs.SetInt(go.name + "Active", go.activeSelf ? 1 : 0);
    }

    private void LoadStatBox(GameObject goDad)
    {
        GameObject go = goDad.transform.GetChild(0).gameObject;
        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.text = PlayerPrefs.GetString(goDad.name);
        goDad.SetActive(PlayerPrefs.GetInt(goDad.name + "Active") == 1);
    }
    private void SaveGameObject(GameObject go)
    {
        PlayerPrefs.SetString(go.name, go.transform.parent.name);
        PlayerPrefs.SetFloat(go.name + "x", go.transform.localPosition.x);
        PlayerPrefs.SetFloat(go.name + "y", go.transform.localPosition.y);
        PlayerPrefs.SetFloat(go.name + "ColorR", go.GetComponent<Image>().color.r);
        PlayerPrefs.SetFloat(go.name + "ColorG", go.GetComponent<Image>().color.g);
        PlayerPrefs.SetFloat(go.name + "ColorB", go.GetComponent<Image>().color.b);
    }

    private void LoadGameObject(GameObject go)
    {
        if (go != null)
        {
            GameObject savedObjectParent = GameObject.Find(PlayerPrefs.GetString(go.name));
            Image image = go.GetComponent<Image>();
            go.transform.SetParent(savedObjectParent.gameObject.transform, false);
            float x = PlayerPrefs.GetFloat(go.name + "x");
            float y = PlayerPrefs.GetFloat(go.name + "y");
            go.transform.localPosition = new Vector2(x, y);
            float r = PlayerPrefs.GetFloat(go.name + "ColorR");
            float g = PlayerPrefs.GetFloat(go.name + "ColorG");
            float b = PlayerPrefs.GetFloat(go.name + "ColorB");
            image.color = new Color(r, g, b);
        }
    }


    private void SaveCardObject(CardObject co)
    {
        GameObject go = co.GameObject;
        PlayerPrefs.SetInt(go.name + "Acted", co.acted ? 1 : 0);
        PlayerPrefs.SetInt(go.name + "Moved", co.moved ? 1 : 0);
        PlayerPrefs.SetInt(go.name + "EffectUsed", co.usedEffect ? 1 : 0);
        PlayerPrefs.SetInt(go.name + "TurnEffectWasUsedOn", co.TurnEffectWasUsedOn);
        //PlayerPrefs.SetInt(go.name + "CurrentHP", co.currentHP);
        SaveGameObject(go);
    }
    private void LoadCardObject(CardObject co)
    {
        GameObject go = co.GameObject;
        co.acted = PlayerPrefs.GetInt(go.name + "Acted") == 1;
        co.moved = PlayerPrefs.GetInt(go.name + "Moved") == 1;
        co.usedEffect = PlayerPrefs.GetInt(go.name + "EffectUsed") == 1;
        co.TurnEffectWasUsedOn = PlayerPrefs.GetInt(go.name + "TurnEffectWasUsedOn");
        //co.currentHP = PlayerPrefs.GetInt(go.name + "CurrentHP");
        LoadGameObject(go);
    }


    private void RestoreCards()
    {
        foreach (CardObject co in CardGameObjectsInGame)
        {
            GameObject go = co.GameObject;
            Image goImage = go.GetComponent<Image>();
            goImage.color = new Color32(255, 255, 255, 255);
            co.acted = false;
            co.usedEffect = false;
            co.moved = false;
        }
    }

    public CardObject CreateCardInSlot(string cardName, GameObject cardSlot)
    {
        Sprite enemyLeaderSprite = (Sprite)Resources.Load("cards/" + cardName, typeof(Sprite));
        GameObject go = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.name = cardName + "Card";
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(112, 160);
        Image image = go.GetComponent<Image>();
        image.sprite = enemyLeaderSprite;
        go.transform.SetParent(cardSlot.transform, false);
        CardZoom script = go.GetComponent<CardZoom>();
        script.ZoomedCard = ZoomedCard;
        Card card = FindCard(cardName);
        CardObject cardObject = new CardObject(go);
        cardObject.card = card;
        cardObject.currentHP = card.HP;
        cardObject.currentATK = card.Attack;
        UpdateStatBoxes(cardObject, cardSlot: cardSlot);
        return cardObject;
    }

    public GameObject CreateCard(string cardName, bool hideCard)
    {
        Sprite sprite;
        if (hideCard)
        {
            sprite = (Sprite)Resources.Load("cards/" + "cardback", typeof(Sprite));
        }
        else
        {
            sprite = (Sprite)Resources.Load("cards/" + cardName, typeof(Sprite));
        }
        GameObject go = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.name = cardName + go.GetInstanceID() + "Card" ;
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(112, 160);
        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        CardZoom script = go.GetComponent<CardZoom>();
        script.ZoomedCard = ZoomedCard;
        return go;
    }

    public void UpdateStatBoxes(CardObject co, GameObject cardSlot, GameObject previousParent = null)
    {
        string slotNumber = cardSlot.name.Substring(8, 2);
        GameObject hpGODad = cardSlot.transform.Find("HPBlock" + slotNumber).gameObject;
        GameObject hpGO = hpGODad.transform.GetChild(0).gameObject;
        GameObject atkGODad = cardSlot.transform.Find("ATKBlock" + slotNumber).gameObject;
        GameObject atkGO = atkGODad.transform.GetChild(0).gameObject;
        hpGO.GetComponent<TextMeshProUGUI>().text = co.currentHP.ToString();
        atkGO.GetComponent<TextMeshProUGUI>().text = co.currentATK.ToString();
        hpGODad.SetActive(true);
        atkGODad.SetActive(true);

        //If it was in another slot and not in the hand
        if(previousParent != null && previousParent.name != "PlayerHandArea")
        {
            string ppslotNumber = previousParent.name.Substring(8, 2);
            GameObject pphpGODad = previousParent.transform.Find("HPBlock" + ppslotNumber).gameObject;
            GameObject ppatkGODad = previousParent.transform.Find("ATKBlock" + ppslotNumber).gameObject;
            pphpGODad.SetActive(false);
            ppatkGODad.SetActive(false);
        }

    }

    private Deck CreateDeck(string leader)
    {
        Deck deck = new Deck();
        deck.InitializeDeck(leader);
        deck.Shuffle();
        return deck;
    }



    public List<Card> Draw(Deck startingdeck, int numberOfCards, Deck.TagType tag = Deck.TagType.All, string cardName = null)
    {
        bool playerDrawing = startingdeck.leaderCard.ImageName == PlayerDeck.leaderCard.ImageName;
        List<Card> ret = new List<Card>();

        for(int i = 0; i< numberOfCards; i++)
        {
            if(startingdeck.cards.Count == 0)
            {
                return ret;
            }
            Card drawnCard = startingdeck.cards.Pop();
            GameObject go = CreateCard(drawnCard.ImageName, false);
            ret.Add(drawnCard);
            if (playerDrawing)
            {
                go.transform.SetParent(PlayerHandArea.transform, false);
            }
            else
            {
                go.transform.SetParent(EnemyHandArea.transform, false);

            }
        }
        return ret;
    }


    private bool SlotWithCard(GameObject go)
    {
        return go.transform.childCount > 3;
    }

    public void RearrangeHand(bool playerHand)
    {
        int i = 0;
        float cardWidth = CardPrefab.transform.GetComponent<RectTransform>().sizeDelta.x;
        GameObject parent;
        float y;
        if (playerHand)
        {
            y = 201;
            parent = PlayerHandArea;
        }
        else
        {
            y = -195;
            parent = EnemyHandArea;
        }
        int cardNumbers = parent.transform.childCount;
        float handWidth = parent.transform.GetComponent<RectTransform>().sizeDelta.x;
        float defaultSeparator = (handWidth - (cardWidth * STARTING_CARDS_HAND)) / STARTING_CARDS_HAND-1;
        float xSeparator = defaultSeparator;
        if (cardNumbers > STARTING_CARDS_HAND)
        {
            xSeparator = (handWidth - (cardWidth * cardNumbers)) / (cardNumbers -1);
        }
        for (int j = 0; j < parent.transform.childCount; j++)
        {
            GameObject go = parent.transform.GetChild(j).gameObject;
            float x = -244 + ((cardWidth + xSeparator) * i);
            
            go.transform.SetParent(parent.transform, false);
            go.transform.localPosition = new Vector3(x, y, 0);
            i++;
        }
    }



    public bool CanAttack(GameObject slot, GameObject defender)
    {
        int locationY = Int32.Parse(slot.name.Substring(8, 1));
        int locationX = Int32.Parse(slot.name.Substring(9, 1));
        int enemyLocationY = Int32.Parse(defender.name.Substring(8, 1));
        int enemyLocationX = Int32.Parse(defender.name.Substring(9, 1));

        //Enemy Card Attacking
        if (locationY < 2)
        {
            if(locationY == 0)
            {
                return false;
            }
            if(locationY == 1)
            {
                if (locationX - enemyLocationX < 2)
                {
                    return true;
                }
            }
        }
        //Player Card Attacking
        else
        {
            if(locationY == 2)
            {
                if(locationX - enemyLocationX < 2 && (enemyLocationY == 1 || (enemyLocationY == 0 && NoEnemyFrontRow(1))))
                {
                    return true;
                }
            }
            if(locationY == 3)
            {
                if (locationX - enemyLocationX < 2 && ((enemyLocationY == 1 && NoEnemyFrontRow(1)) || (enemyLocationY == 0 && NoEnemyFrontRow(1) && NoEnemyFrontRow(2))))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool NoEnemyFrontRow(int rowsNumber)
    {
        if(rowsNumber == 1)
        {
            return !SlotWithCard(CardSlot10) && !SlotWithCard(CardSlot11) && !SlotWithCard(CardSlot12);
        }
        else
        {
            return !SlotWithCard(CardSlot20) && !SlotWithCard(CardSlot21) && !SlotWithCard(CardSlot22);
        }
    }


    public void AddCounter(CardObject co, int numberOfCounters)
    {
        GameObject cardCounterPanel = Instantiate(CountersPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        GameObject imageObject = cardCounterPanel.transform.GetChild(0).gameObject;
        imageObject.transform.localPosition = new Vector3(-35, 55, 0);
        imageObject.transform.GetComponent<RectTransform>().sizeDelta = new Vector3(30, 30, 0);
        GameObject plusObject = imageObject.transform.GetChild(0).gameObject;
        GameObject counterObject = imageObject.transform.GetChild(1).gameObject;

        plusObject.transform.localPosition = new Vector3((float)-8.5, (float)-0.1, 0);
        plusObject.transform.GetComponent<RectTransform>().sizeDelta = new Vector3(13, 30, 0);
        counterObject.transform.localPosition = new Vector3(7, 0, 0);
        counterObject.transform.GetComponent<RectTransform>().sizeDelta = new Vector3(17, 30, 0);

        TextMeshProUGUI counterText = counterObject.GetComponent<TextMeshProUGUI>();
        co.counters += numberOfCounters;
        co.currentHP += numberOfCounters;
        co.currentATK += numberOfCounters;
        counterText.text = co.counters.ToString();
        cardCounterPanel.transform.SetParent(co.GameObject.transform, false);

        UpdateStatBoxes(co, co.GameObject.transform.parent.gameObject);
    }

}
