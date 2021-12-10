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
    public List<CardObject> CardObjectsInGame = new List<CardObject>();
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
    public int Turn;

    public static GameStart INSTANCE = null;

    //TODO: move to a leader class
    public string EnemyLeader, PlayerLeader;


    // Start is called before the first frame update
    void Start()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        Debug.Log("Starting");
        Turn = 1;
        EffectListener effectListener = new EffectListener();
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
        CardObjectsInGame.Add(leaderCardObject);
        CardObject enemyLeaderCardObject = CreateCardInSlot(EnemyLeader, CardSlot11);
        CardObjectsInGame.Add(enemyLeaderCardObject);
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


    internal void DamageAllEnemyCards(int damage, bool playerCard)
    {
        List<CardObject> iteratable = new List<CardObject>(CardObjectsInGame);
        String fieldName;
        if (playerCard)
        {
            fieldName = "EnemyField";
        }
        else
        {
            fieldName = "PlayerField";
        }
        foreach(CardObject co in iteratable)
        {
            if(co.GameObject.transform.parent.parent.name == fieldName)
            {
                DamageCard(co, damage);
            }
        }
    }

    private void DamageCard(CardObject co, int damage)
    {
        co.currentHP -= damage;
        if (co.currentHP <= 0)
        {
            DestroyCard(co);
        }
    }

    public void UpdateAllStatBoxes()
    {
        foreach(GameObject slot in GetSlotMap().Values)
        {
            if (SlotWithCard(slot))
            {
                GameObject cardGameObject = GetCardGameObject(slot);
                CardObject co = FindCardObject(cardGameObject);
                if(co.card.ImageName == EnemyLeader)
                {
                    
                }
                UpdateStatBoxes(co, slot);
            }
            else
            {
                GameObject hpBox = slot.transform.GetChild(0).gameObject;
                GameObject atkBox = slot.transform.GetChild(1).gameObject;
                hpBox.SetActive(false);
                atkBox.SetActive(false);
            }
        }
    }

    public void CreateShield(GameObject go)
    {
        Sprite shieldSprite = (Sprite)Resources.Load("RonoveShield", typeof(Sprite));
        GameObject gos = new GameObject("Shield");
        RectTransform rectTransform = gos.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(112, 160);
        Image shieldImage = gos.AddComponent<Image>();
        shieldImage.sprite = shieldSprite;
        gos.transform.SetParent(go.transform, false);
        GameObject counterPanel = GetCounterPanel(go);
        if (counterPanel!=null)
        {
            gos.transform.SetSiblingIndex(1);
            counterPanel.transform.SetSiblingIndex(2);
        }

    }

    public GameObject GetCardGameObject(GameObject slot)
    {
        for(int i=0; i<slot.transform.childCount; i++)
        {
            GameObject child = slot.transform.GetChild(i).gameObject;
            if (IsCard(child))
            {
                return child;
            }
        }
        return null;
    }

    public bool IsCard(GameObject card)
    {
        return card.name.EndsWith("Card");
    }

    public void RemoveShield(GameObject go)
    {
        GameObject shield = null;
        for(int i=0; i<go.transform.childCount; i++)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            if (child.name == "Shield")
            {
                shield = child;
                break;
            }
        }
        GameObject.DestroyImmediate(shield);
    }

    public bool HasShield(GameObject go)
    {
        GameObject shield = null;
        for (int i = 0; i < go.transform.childCount; i++)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            if (child.name == "Shield")
            {
                shield = child;
                break;
            }
        }
        return shield != null;
    }


    private void DestroyCard(CardObject co)
    {
        CardObjectsInGame.Remove(co);
        GameObject graveyard;
        if (co.GameObject.transform.parent.parent.name == "PlayerField")
        {
            graveyard = PlayerGraveyard;
        }
        else
        {
            graveyard = EnemyGraveyard;
        }
        co.GameObject.transform.SetParent(graveyard.transform, false);
    }

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

    internal void Defeat()
    {
        EndTurnButton.gameObject.SetActive(false);
        UndoButton.gameObject.SetActive(false);
        Debug.Log("Get rekt m8");
    }

    internal List<CardObject> FindCardsInGameByTag(Deck.TagType stake)
    {
        List<CardObject> ret = new List<CardObject>();
        foreach(CardObject co in CardObjectsInGame)
        {
            if (co.card.Tags.Contains(stake))
            {
                ret.Add(co);
            }
        }
        return ret;
    }

    internal List<CardObject> FindCardObject(string cardName)
    {
        List<CardObject> co = new List<CardObject>();
        foreach (CardObject coin in CardObjectsInGame)
        {
            if (coin.card.ImageName == cardName)
            {
                co.Add(coin);
            }
        }
        return co;
    }

    public CardObject FindCardObject(GameObject go)
    {
        CardObject co = null;
        foreach (CardObject c in CardObjectsInGame)
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
        foreach (CardObject c in CardObjectsInGame)
        {
            if (c.GameObject.name == leaderName + "Card")
            {
                co = c;
                break;
            }
        }
        return co;
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
    }

    public GameObject GetCounterPanel(GameObject go)
    {
        for (int i = 0; i < go.transform.childCount; i++)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            if (child.name.Contains("CounterPanel"))
            {
                return child;
            }
        }
        return null;
    }

    //Returns true if the attack destroys the card
    public bool Attack(GameObject defenderSlot, int damage)
    {
        GameObject cardObject = GetCardGameObject(defenderSlot);
        if (HasShield(cardObject))
        {
            RemoveShield(cardObject);
            return false;
        }
        CardObject co = FindCardObject(cardObject);
        GameObject hpbox = defenderSlot.transform.GetChild(0).GetChild(0).gameObject;
        TextMeshProUGUI hpText = hpbox.GetComponent<TextMeshProUGUI>();
        int newHP = (Int32.Parse(hpText.text) - damage);
        if(newHP <= 0)
        {
            GameObject atkbox = defenderSlot.transform.GetChild(1).GetChild(0).gameObject;
            hpText.text = 0.ToString();
            co.currentHP = 0;
            //if attacking leader
            if (EnemyLeader == cardObject.GetComponent<Image>().sprite.name)
            {
                EnemyLeaderImage.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = 0.ToString();
                Image playerLeaderImage = PlayerLeaderImage.transform.GetChild(1).GetComponent<Image>();
                Image enemyLeaderImage = EnemyLeaderImage.transform.GetChild(1).GetComponent<Image>();
                Sprite playerLeaderSprite = (Sprite)Resources.Load("Leaders/" + PlayerLeader + "3", typeof(Sprite));
                playerLeaderImage.sprite = playerLeaderSprite;
                Sprite enemyLeaderSprite = (Sprite)Resources.Load("Leaders/" + EnemyLeader + "1", typeof(Sprite));
                enemyLeaderImage.sprite = enemyLeaderSprite;
            }
            else if (PlayerLeader == cardObject.GetComponent<Image>().sprite.name)
            {
                PlayerLeaderImage.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = 0.ToString();
                Image playerLeaderImage = PlayerLeaderImage.transform.GetChild(1).GetComponent<Image>();
                Image enemyLeaderImage = EnemyLeaderImage.transform.GetChild(1).GetComponent<Image>();
                Sprite playerLeaderSprite = (Sprite)Resources.Load("Leaders/" + PlayerLeader + "1", typeof(Sprite));
                playerLeaderImage.sprite = playerLeaderSprite;
                Sprite enemyLeaderSprite = (Sprite)Resources.Load("Leaders/" + EnemyLeader + "3", typeof(Sprite));
                enemyLeaderImage.sprite = enemyLeaderSprite;
            }
            return true;
        }
        else
        {
            hpText.text = newHP.ToString();
            co.currentHP = newHP;
            if (EnemyLeader == GetCardGameObject(defenderSlot).GetComponent<Image>().sprite.name)
            {
                EnemyLeaderImage.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = newHP.ToString();
                if (newHP < EnemyDeck.leaderCard.HP / 3)
                {
                    Image playerLeaderImage = PlayerLeaderImage.transform.GetChild(1).GetComponent<Image>();
                    Image enemyLeaderImage = EnemyLeaderImage.transform.GetChild(1).GetComponent<Image>();
                    Sprite playerLeaderSprite = (Sprite)Resources.Load("Leaders/" + PlayerLeader + "3", typeof(Sprite));
                    playerLeaderImage.sprite = playerLeaderSprite;
                    Sprite enemyLeaderSprite = (Sprite)Resources.Load("Leaders/" + EnemyLeader + "1", typeof(Sprite));
                    enemyLeaderImage.sprite = enemyLeaderSprite;
                }
            }
            else if (PlayerLeader == cardObject.GetComponent<Image>().sprite.name)
            {
                PlayerLeaderImage.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = newHP.ToString();
                if(newHP < PlayerDeck.leaderCard.HP/3)
                {
                    Image playerLeaderImage = PlayerLeaderImage.transform.GetChild(1).GetComponent<Image>();
                    Image enemyLeaderImage = EnemyLeaderImage.transform.GetChild(1).GetComponent<Image>();
                    Sprite playerLeaderSprite = (Sprite)Resources.Load("Leaders/" + PlayerLeader + "1", typeof(Sprite));
                    playerLeaderImage.sprite = playerLeaderSprite;
                    Sprite enemyLeaderSprite = (Sprite)Resources.Load("Leaders/" + EnemyLeader + "3", typeof(Sprite));
                    enemyLeaderImage.sprite = enemyLeaderSprite;
                }
                
            }
            return false;
        }
    }

    internal void Victory()
    {
        EndTurnButton.gameObject.SetActive(false);
        UndoButton.gameObject.SetActive(false);
        Debug.Log("GG");
    }

    private void OnEndTurn()
    {
        if (GameState != State.EnemyTurn)
        {
            GameState = State.EnemyTurn;
            CardZoom.RemovePreviousMark();
            EffectListener.INSTANCE.OnTrunEnd();
            AIFunctions.INSTANCE.TakeTurn(EnemyDeck);
            Turn++;
        }
    }


    public void OnTurnStart()
    {
        GameState = State.Moving;
        if (PlayerHand.cards.Count < MAX_CARDS_HAND)
        {
            List<Card> cardsDrawn = Draw(PlayerDeck, 1);
            if(cardsDrawn.Count != 0)
            {
                Card drawnCard = cardsDrawn[0];
                PlayerHand.cards.Add(drawnCard);
                RearrangeHand(true);
            }
        }
        RestoreCards();
        SaveState();
    }


    private void SaveState()
    {
        for(int i=0; i< PlayerHandArea.transform.childCount; i++)
        {
            GameObject go = PlayerHandArea.transform.GetChild(i).gameObject;
            SaveGameObject(go);
        }
        foreach (CardObject co in CardObjectsInGame)
        {
            SaveCardObject(co);
        }
        foreach (GameObject go in StatBoxes)
        {
            SaveStatBox(go);
        }
        for (int i = 0; i < EnemyGraveyard.transform.childCount; i++)
        {
            GameObject go = EnemyGraveyard.transform.GetChild(i).gameObject;
            SaveGameObject(go);
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
            List<CardObject> iteratableList = new List<CardObject>(CardObjectsInGame);
            foreach (CardObject go in iteratableList)
            {
                LoadCardObject(go);
            }
            for (int i = 0; i < EnemyGraveyard.transform.childCount; i++)
            {
                GameObject go = EnemyGraveyard.transform.GetChild(i).gameObject;
                LoadGameObject(go);
                if (go.transform.parent.name != "EnemyGraveyard")
                {
                    Card c = FindCard(go.GetComponent<Image>().sprite.name);
                    CardObject co = new CardObject(go, c);
                    CardObjectsInGame.Add(co);
                }
            }
            UpdateCounters();
            HidePosibleMovements();
            PlayerLifePoints.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("PlayerLifePoints");
            EnemyLifePoints.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("EnemyLifePoints");
            GameState = State.Moving;
            CardUsingEffect = null;
            CardZoom.RemovePreviousMark();
        }
    }

    private void UpdateCounters()
    {
        foreach (CardObject co in CardObjectsInGame)
        {
            GameObject counterPanel = GetCounterPanel(co.GameObject);
            if (co.counters == 0 && co.GameObject.transform.childCount > 0)
            {
                if (counterPanel != null)
                {
                    GameObject.Destroy(counterPanel);
                }
            }
            else
            {
                if (counterPanel != null)
                {
                    TextMeshProUGUI counterText = GetCounterText(counterPanel);
                    counterText.text = co.counters.ToString();
                }
            }
        }
    }

    private TextMeshProUGUI GetCounterText(GameObject counterPanel)
    {
        GameObject imageObject = counterPanel.transform.GetChild(0).gameObject;
        GameObject counterObject = imageObject.transform.GetChild(1).gameObject;
        TextMeshProUGUI counterText = counterObject.GetComponent<TextMeshProUGUI>();
        return counterText;
    }

    public void UseCardEffect(CardObject co, GameObject objective)
    {
        Debug.Log("Using " + co.card.ImageName + "'s effect");
        Card c = co.card;
        c.InitializeEffectParametrs();
        c.SetTargetCard(objective);
        bool effectSuccess = co.card.Effect(c);
        if (effectSuccess)
        {
            co.usedEffect = true;
            co.TurnEffectWasUsedOn = Turn;
        }
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
            //If it was a card drawn this turn by some effect
            if(savedObjectParent == null)
            {
                ShuffleCardBackInDeck(go, PlayerDeck);
                CardObject co = FindCardObject(go);
                if (co != null)
                {
                    CardObjectsInGame.Remove(co);
                }
                GameObject.Destroy(go);
                return;
            }
            else if(savedObjectParent.name == "PlayerHandArea")
            {
                CardObject co = FindCardObject(go);
                if (co != null)
                {
                    CardObjectsInGame.Remove(co);
                    for (int i = 0; i < go.transform.childCount; i++)
                    {
                        GameObject child = go.transform.GetChild(i).gameObject;
                        if (child.name.StartsWith("CounterPanel"))
                        {
                            GameObject.DestroyImmediate(child);
                            break;
                        }
                    }
                }
                
            }
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

    private void ShuffleCardBackInDeck(GameObject go, Deck deck)
    {
        string cardName = go.name.Split('-')[0];
        Card c = FindCard(cardName);
        deck.cards.Push(c);
    }

    private void SaveCardObject(CardObject co)
    {
        GameObject go = co.GameObject;
        PlayerPrefs.SetInt(go.name + "Acted", co.acted ? 1 : 0);
        PlayerPrefs.SetInt(go.name + "Moved", co.moved ? 1 : 0);
        PlayerPrefs.SetInt(go.name + "EffectUsed", co.usedEffect ? 1 : 0);
        PlayerPrefs.SetInt(go.name + "TurnEffectWasUsedOn", co.TurnEffectWasUsedOn);
        PlayerPrefs.SetInt(go.name + "Counters", co.counters);
        PlayerPrefs.SetInt(go.name + "CurrentHP", co.currentHP);
        int hasShield = HasShield(co.GameObject)?1:0;
        PlayerPrefs.SetInt(go.name + "HasShield", hasShield);
        SaveGameObject(go);
    }
    private void LoadCardObject(CardObject co)
    {
        GameObject go = co.GameObject;
        co.acted = PlayerPrefs.GetInt(go.name + "Acted") == 1;
        co.moved = PlayerPrefs.GetInt(go.name + "Moved") == 1;
        co.usedEffect = PlayerPrefs.GetInt(go.name + "EffectUsed") == 1;
        co.TurnEffectWasUsedOn = PlayerPrefs.GetInt(go.name + "TurnEffectWasUsedOn");
        co.currentHP = PlayerPrefs.GetInt(go.name + "CurrentHP");
        co.counters = PlayerPrefs.GetInt(go.name + "Counters");
        bool hasShield = PlayerPrefs.GetInt(go.name + "HasShield")==1;
        if (hasShield && !HasShield(co.GameObject))
        {
            CreateShield(co.GameObject);
        }
        else if(!hasShield && HasShield(co.GameObject))
        {
            RemoveShield(co.GameObject);
        }

        LoadGameObject(go);
    }

    public GameObject GetEmptySlot(GameObject field)
    {
        for(int i =0; i<field.transform.childCount; i++)
        {
            GameObject slot = field.transform.GetChild(i).gameObject;
            if (!SlotWithCard(slot))
            {
                return slot;
            }
        }
        return null;
    }


    private void RestoreCards()
    {
        foreach (CardObject co in CardObjectsInGame)
        {
            GameObject go = co.GameObject;
            Image goImage = go.GetComponent<Image>();
            goImage.color = new Color32(255, 255, 255, 255);
            co.acted = false;
            co.usedEffect = false;
            co.moved = false;
        }
    }

    public CardObject PlayCardInSlot(string cardName, GameObject cardSlot)
    {
        Card card = FindCard(cardName);
        GameObject cardInHand = null;
        for(int index = 0; index < EnemyHandArea.transform.childCount; index++)
        {
            if(EnemyHandArea.transform.GetChild(index).GetComponent<Image>().sprite.name == cardName)
            {
                cardInHand = EnemyHandArea.transform.GetChild(index).gameObject;
                GameObject cardBackInEnemyCards = cardInHand.transform.GetChild(0).gameObject;
                GameObject.Destroy(cardBackInEnemyCards);
                break;
            }
        }
        CardObject cardObject = new CardObject(cardInHand, card);
        cardInHand.transform.SetParent(cardSlot.transform, false);
        cardInHand.transform.localPosition = new Vector3(0, 0, 0);
        UpdateStatBoxes(cardObject, cardSlot: cardSlot);
        return cardObject;
    }

    public CardObject CreateCardInSlot(string cardName, GameObject cardSlot)
    {
        Sprite enemyLeaderSprite = (Sprite)Resources.Load("cards/" + cardName, typeof(Sprite));
        GameObject go = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(112, 160);
        Image image = go.GetComponent<Image>();
        image.sprite = enemyLeaderSprite;
        go.transform.SetParent(cardSlot.transform, false);
        CardZoom script = go.GetComponent<CardZoom>();
        script.ZoomedCard = ZoomedCard;
        Card card = FindCard(cardName);
        CardObject cardObject = new CardObject(go, card);
        if (card.Tags.Contains(Deck.TagType.Leader))
        {
            go.name = cardName + "Card";
        }
        else
        {
            go.name = cardName + go.GetInstanceID() + "Card";
        }
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
        if (!CardObjectsInGame.Contains(co) && previousParent != null)
        {
            string ppslotNumber = previousParent.name.Substring(8, 2);
            GameObject pphpGODad = previousParent.transform.Find("HPBlock" + ppslotNumber).gameObject;
            GameObject ppatkGODad = previousParent.transform.Find("ATKBlock" + ppslotNumber).gameObject;
            pphpGODad.SetActive(false);
            ppatkGODad.SetActive(false);
        }
        if (cardSlot != null)
        {
            string slotNumber = cardSlot.name.Substring(8, 2);
            GameObject hpGODad = cardSlot.transform.Find("HPBlock" + slotNumber).gameObject;
            GameObject atkGODad = cardSlot.transform.Find("ATKBlock" + slotNumber).gameObject;
            GameObject hpGO = hpGODad.transform.GetChild(0).gameObject;
            GameObject atkGO = atkGODad.transform.GetChild(0).gameObject;
            hpGO.GetComponent<TextMeshProUGUI>().text = co.currentHP.ToString();
            atkGO.GetComponent<TextMeshProUGUI>().text = (co.counters + co.card.Attack).ToString();
            hpGODad.SetActive(true);
            atkGODad.SetActive(true);
        }

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
            Card drawnCard;
            if (tag != Deck.TagType.All)
            {
                drawnCard = GetCardInDeckByTag(tag);
                if(drawnCard == null)
                {
                    return new List<Card>();
                }
            }
            else if (cardName != null)
            {
                drawnCard = GetCardInDeckByName(startingdeck, cardName);
                if (drawnCard == null)
                {
                    return new List<Card>();
                }
            }
            else
            {
                if(startingdeck.cards.Count == 0)
                {
                    return new List<Card>();
                }
                drawnCard = startingdeck.cards.Pop();
            }
            GameObject go = CreateCard(drawnCard.ImageName, false);
            ret.Add(drawnCard);
            if (playerDrawing)
            {
                go.transform.SetParent(PlayerHandArea.transform, false);
            }
            else
            {
                GameObject cardBack = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                Sprite cardBackSprite = (Sprite)Resources.Load("cards/" + "cardback", typeof(Sprite));
                Image image = cardBack.GetComponent<Image>();
                image.sprite = cardBackSprite;
                go.transform.SetParent(EnemyHandArea.transform, false);
                CardZoom script = cardBack.GetComponent<CardZoom>();
                script.ZoomedCard = ZoomedCard;
                cardBack.transform.SetParent(go.transform, false);

            }
        }
        return ret;
    }

    public void HidePosibleMovements()
    {
        for (int i = 0; i < PlayerField.transform.childCount; i++)
        {
            GameObject slot = PlayerField.transform.GetChild(i).gameObject;
            for (int j = 0; j < slot.transform.childCount; j++)
            {
                GameObject slotChild = slot.transform.GetChild(j).gameObject;
                if (slotChild.name == "MovementGuide")
                {
                    GameObject.Destroy(slotChild);
                }

            }
        }
    }

    private Card GetCardInDeckByName(Deck deck, string cardName)
    {
        Card drawnCard = null;
        Stack<Card> newStack = new Stack<Card>();
        while (deck.cards.Count != 0)
        {
            Card card = deck.cards.Pop();
            if (card.ImageName == cardName && drawnCard == null)
            {
                drawnCard = card;
            }
            else
            {
                newStack.Push(card);
            }
        }
        deck.cards = newStack;
        return drawnCard;
    }

    private Card GetCardInDeckByTag(Deck.TagType tag)
    {
        Card drawnCard = null;
        Stack<Card> newStack = new Stack<Card>();
        Deck deck = PlayerDeck;
        while (deck.cards.Count != 0)
        {
            Card card = deck.cards.Pop();
            if (card.Tags.Contains(Deck.TagType.Stake) && drawnCard == null)
            {
                drawnCard = card;
            }
            else
            {
                newStack.Push(card);
            }
        }
        deck.cards = newStack;
        return drawnCard;
    }

    public bool SlotWithCard(GameObject go)
    {
        return go.transform.childCount > 3;
    }

    public void RearrangeHand(bool playerHand)
    {
        int i = 0;
        float cardWidth = CardPrefab.transform.GetComponent<RectTransform>().sizeDelta.x;
        GameObject handArea;
        float y;
        if (playerHand)
        {
            y = 201;
            handArea = PlayerHandArea;
        }
        else
        {
            y = -195;
            handArea = EnemyHandArea;
        }
        int cardNumbers = handArea.transform.childCount;
        float handWidth = handArea.transform.GetComponent<RectTransform>().sizeDelta.x;
        float defaultSeparator = (handWidth - (cardWidth * STARTING_CARDS_HAND)) / STARTING_CARDS_HAND-1;
        float xSeparator = defaultSeparator;
        if (cardNumbers > STARTING_CARDS_HAND)
        {
            xSeparator = (handWidth - (cardWidth * cardNumbers)) / (cardNumbers -1);
        }
        for (int j = 0; j < handArea.transform.childCount; j++)
        {
            GameObject go = handArea.transform.GetChild(j).gameObject;
            float x = -244 + ((cardWidth + xSeparator) * i);
            
            go.transform.SetParent(handArea.transform, false);
            go.transform.localPosition = new Vector3(x, y, 0);
            i++;
        }
    }



    public bool CanAttack(GameObject slot, GameObject defender)
    {
        GameObject go = GetCardGameObject(slot);
        CardObject co = FindCardObject(go);
        if (EffectListener.INSTANCE.CanAttackFromAnywhereList.Contains(co.card))
        {
            return true;
        }
        int locationY = Int32.Parse(slot.name.Substring(8, 1));
        int locationX = Int32.Parse(slot.name.Substring(9, 1));
        int enemyLocationY = Int32.Parse(defender.name.Substring(8, 1));
        int enemyLocationX = Int32.Parse(defender.name.Substring(9, 1));

        //Enemy Card Attacking
        if (locationY < 2)
        {
            if ((locationX - enemyLocationX < 2) && (locationX - enemyLocationX > -2) && ((enemyLocationY == 2 && NoCardInRow(1)) || (enemyLocationY == 3 && NoCardInRow(2) && NoCardInRow(1))))
            {
                return true;
            }
            if (locationY == 1)
            {
                if ((locationX - enemyLocationX < 2) && (locationX - enemyLocationX > -2) && (enemyLocationY == 2|| (enemyLocationY == 3 && NoCardInRow(2))))
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
                if((locationX - enemyLocationX < 2) && (locationX - enemyLocationX > -2) && (enemyLocationY == 1 || (enemyLocationY == 0 && NoCardInRow(1))))
                {
                    return true;
                }
            }
            if(locationY == 3)
            {
                if ((locationX - enemyLocationX < 2) && (locationX - enemyLocationX > -2) && ((enemyLocationY == 1 && NoCardInRow(2)) || (enemyLocationY == 0 && NoCardInRow(1) && NoCardInRow(2))))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool NoCardInRow(int rowsNumber)
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

    public void AddCounterEffect(CardObject co, int numberOfCounters)
    {
        AddCounter(co, numberOfCounters);
        EffectListener.INSTANCE.OnGettingCounters(co.GameObject, 1);
    }

    internal void RemoveCounter(CardObject co, int numberOfCounters)
    {
        GameObject cardCounterPanel = co.GameObject.transform.GetChild(0).gameObject;
        GameObject imageObject = cardCounterPanel.transform.GetChild(0).gameObject;
        GameObject counterObject = imageObject.transform.GetChild(1).gameObject;

        TextMeshProUGUI counterText = counterObject.GetComponent<TextMeshProUGUI>();
        co.counters -= numberOfCounters;
        if(co.counters == 0)
        {
            GameObject.DestroyImmediate(cardCounterPanel);
        }
        else
        {
            counterText.text = co.counters.ToString();
        }
        co.currentHP -= numberOfCounters;
        co.currentATK -= numberOfCounters;
        UpdateStatBoxes(co, co.GameObject.transform.parent.gameObject);
    }


    public void AddCounter(CardObject co, int numberOfCounters)
    {
        GameObject counterPanel = GetCounterPanel(co.GameObject);
        TextMeshProUGUI counterText = null;
        if (counterPanel != null)
        {
            counterText = GetCounterText(counterPanel);
        }
        else
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
            counterText = counterObject.GetComponent<TextMeshProUGUI>();
            cardCounterPanel.transform.SetParent(co.GameObject.transform, false);
        }
        co.counters += numberOfCounters;
        co.currentHP += numberOfCounters;
        co.currentATK += numberOfCounters;
        counterText.text = co.counters.ToString();
        UpdateStatBoxes(co, co.GameObject.transform.parent.gameObject);
    }

    public GameObject FindFreeSlot(bool enemyField)
    {
        if (enemyField)
        {
            for(int i = 0; i < EnemyField.transform.childCount; i++)
            {
                GameObject slot = EnemyField.transform.GetChild(i).gameObject;
                bool hasCard = SlotWithCard(slot);
                if (!hasCard)
                {
                    return slot;
                }
            }
        }
        return null;
    }
}
