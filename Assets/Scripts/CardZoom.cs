using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static Deck;
using Image = UnityEngine.UI.Image;

public class CardZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject ZoomedCard;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //In case the game is minimized and you hover over something
        if(this == null)
        {
            return;
        }
        //If theres a card in the hovered slot or its a card in hand
        if((IsCard(this.gameObject) && this.transform.parent.name != "EnemyHandArea") || this.transform.parent.name == "PlayerHandArea")
        {
            CardObject co = GameStart.INSTANCE.FindCardObject(this.gameObject);
            if (co == null)
            {
                return;
            }
            if(co!=null && co.counters > 0)
            {
                GameObject cardCounterPanel = Instantiate(GameStart.INSTANCE.CountersPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                GameObject imageObject = cardCounterPanel.transform.GetChild(0).gameObject;
                imageObject.transform.localPosition = new Vector3(-125, 205, 0);
                imageObject.transform.GetComponent<RectTransform>().sizeDelta = new Vector3(80, 80, 0);
                GameObject plusObject = imageObject.transform.GetChild(0).gameObject;
                GameObject counterObject = imageObject.transform.GetChild(1).gameObject;

                plusObject.transform.localPosition = new Vector3(-27, (float)2.5, 0);
                plusObject.transform.GetComponent<RectTransform>().sizeDelta = new Vector3(25, 80, 0);
                counterObject.transform.localPosition = new Vector3(13, (float)7.5, 0);
                counterObject.transform.GetComponent<RectTransform>().sizeDelta = new Vector3(55, 80, 0);

                TextMeshProUGUI counterText = counterObject.GetComponent<TextMeshProUGUI>();
                counterText.text = co.counters.ToString();
                cardCounterPanel.transform.SetParent(ZoomedCard.transform, false);

            }

            Sprite sprite = this.GetComponent<Image>().sprite;
            ZoomedCard.GetComponent<Image>().sprite = sprite;
            ZoomedCard.SetActive(true);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ZoomedCard!=null && ZoomedCard.transform.childCount > 0)
        {
            GameObject child = ZoomedCard.transform.GetChild(0).gameObject;
            GameObject.Destroy(child);
        }
        ZoomedCard.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerClick.name);
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            try
            {
                string cardName = this.GetComponent<Image>().sprite.name;
                CardObject cardObject = GameStart.INSTANCE.FindCardObject(this.gameObject);
                if (IsAllyCard(eventData.pointerClick))
                {
                    if (cardObject == null || (GameStart.INSTANCE.CardUsingEffect!=null && cardObject.card.PassiveEffect))
                    {
                        return;
                    }
                    if (IsCardInHand(cardObject))
                    {
                        return;
                    }
                    if (!cardObject.usedEffect && (GameStart.INSTANCE.GameState == GameStart.State.Moving || GameStart.INSTANCE.GameState == GameStart.State.Summoning))
                    {
                        if (cardObject.card.Cooldown == 0 || cardObject.TurnEffectWasUsedOn == 0 || (cardObject.TurnEffectWasUsedOn != 0 && (cardObject.card.Cooldown + cardObject.TurnEffectWasUsedOn <= GameStart.INSTANCE.Turn)))
                        {
                            if (!cardObject.card.UsesTarget)
                            {
                                GameStart.INSTANCE.UseCardEffect(cardObject, null);
                                Debug.Log("Using effect");
                            }
                            else
                            {
                                CreateMark();
                                GameStart.INSTANCE.CardUsingEffect = cardObject.card;
                                Debug.Log("Using effect");
                            }
                        }
                        else
                        {
                            Debug.Log("This card needs to wait " + cardObject.card.Cooldown.ToString() + "+" + cardObject.TurnEffectWasUsedOn + "<=" + GameStart.INSTANCE.Turn);
                            return;
                        }
                    }
                    else
                    {
                        Debug.Log("Already used this effect");
                    }
                }
                else
                {
                    Debug.Log("Can't use enemy effects");
                }
                
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No card");
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (GameStart.INSTANCE.SelectedCardGameObject == null && IsAllyCard(eventData.pointerClick) && GameStart.INSTANCE.CardUsingEffect == null)
            {
                CreateMark();
            }
            else if (IsAllyCard(eventData.pointerClick) && GameStart.INSTANCE.CardUsingEffect==null)
            {
                CreateMark();
            }
            else
            {
                if (GameStart.INSTANCE.SelectedCardGameObject == null)
                {
                    return;
                }
                if (eventData.pointerClick.name == GameStart.INSTANCE.SelectedCardGameObject.name)
                {
                    RemovePreviousMark();
                    GameStart.INSTANCE.CardUsingEffect = null;
                }
                else
                {
                    if (GameStart.INSTANCE.CardUsingEffect != null)
                    {
                        CardObject co = GameStart.INSTANCE.FindCardObject(eventData.pointerClick);
                        if(co == null)
                        {
                            return;
                        }
                        TargetType effectType = co.card.TargetType;
                        TagType targetTag = GameStart.INSTANCE.CardUsingEffect.TargetTag;
                        if((effectType == TargetType.Ally && IsAllyCard(co.GameObject))
                         ||(effectType == TargetType.Enemy && IsEnemyCard(co.GameObject))
                         || effectType == TargetType.Both && co.GameObject.transform.parent.name != "EnemyHandArea"
                         && (targetTag == TagType.All || co.card.Tags.Contains(targetTag)))
                        {
                            UseEffect(eventData.pointerClick);
                        }
                        else
                        {
                            Debug.Log("Targeting wrong card to use the effect on");
                        }
                    }
                    else
                    {
                        if (CardInHand(GameStart.INSTANCE.SelectedCardGameObject) && PlayerOpenSlot(eventData.pointerClick) && (GameStart.INSTANCE.GameState == GameStart.State.Summoning || GameStart.INSTANCE.GameState == GameStart.State.Moving))
                        {
                            GameObject movingCard = GameStart.INSTANCE.SelectedCardGameObject;
                            string cardName = movingCard.GetComponent<Image>().sprite.name;
                            Card card = GameStart.INSTANCE.FindCard(cardName);
                            GameObject costBlock = eventData.pointerClick.transform.GetChild(2).GetChild(0).gameObject;
                            int costInSlot = Int32.Parse(costBlock.GetComponent<TextMeshProUGUI>().text);
                            if (card.Cost <= costInSlot)
                            {
                                GameStart.INSTANCE.PlayerHand.cards.Remove(card);
                                GameStart.INSTANCE.GameState = GameStart.State.Summoning;
                                GameObject previousParent = movingCard.transform.parent.gameObject;
                                movingCard.transform.SetParent(eventData.pointerClick.gameObject.transform, false);
                                movingCard.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                                CardObject co = new CardObject(movingCard);
                                co.currentHP = card.HP;
                                co.currentATK = card.Attack;
                                co.card = card;
                                GameStart.INSTANCE.CardGameObjectsInGame.Add(co);
                                EffectListener.INSTANCE.OnCardPlayed(co);
                                GameStart.INSTANCE.UpdateStatBoxes(co, eventData.pointerClick.gameObject, previousParent);
                                RemovePreviousMark();
                                GameStart.INSTANCE.RecalculateCosts();
                                GameStart.INSTANCE.RearrangeHand(true);
                                Debug.Log("Put card in field");
                            }
                            else
                            {
                                Debug.Log("Cost too high for card");
                            }

                        }
                        else if (PlayerCardInGame(GameStart.INSTANCE.SelectedCardGameObject) && PlayerOpenSlot(eventData.pointerClick) && GameStart.INSTANCE.GameState == GameStart.State.Moving)
                        {
                            GameObject movingCard = GameStart.INSTANCE.SelectedCardGameObject;
                            CardObject cardObject = GameStart.INSTANCE.FindCardObject(movingCard);
                            if (cardObject.moved)
                            {
                                Debug.Log("Already moved");
                                return;
                            }
                            bool canMove = CanMoveToPosition(movingCard, eventData.pointerClick);
                            if (!canMove)
                            {
                                Debug.Log("Cant move to this spot");
                                return;
                            }
                            cardObject.moved = true;
                            GameObject previousParent = movingCard.transform.parent.gameObject;
                            movingCard.transform.SetParent(eventData.pointerClick.gameObject.transform, false);
                            GameStart.INSTANCE.UpdateStatBoxes(cardObject, eventData.pointerClick.gameObject, previousParent);
                            RemovePreviousMark();
                            GameStart.INSTANCE.RecalculateCosts();
                            Debug.Log("Card moves");
                        }
                        else if (PlayerCardInGame(GameStart.INSTANCE.SelectedCardGameObject) && EnemyCardInGame(eventData.pointerClick) && (GameStart.INSTANCE.GameState == GameStart.State.Summoning || GameStart.INSTANCE.GameState == GameStart.State.Moving || GameStart.INSTANCE.GameState == GameStart.State.Battle))
                        {
                            string playerCardName = GameStart.INSTANCE.SelectedCardGameObject.GetComponent<Image>().sprite.name;
                            Card playerCard = GameStart.INSTANCE.FindCard(playerCardName);
                            CardObject cardObject = GameStart.INSTANCE.FindCardObject(GameStart.INSTANCE.SelectedCardGameObject);
                            CardObject enemyCardObject = GameStart.INSTANCE.FindCardObject(eventData.pointerClick);
                            if (!GameStart.INSTANCE.CanAttack(GameStart.INSTANCE.SelectedCardGameObject.transform.parent.gameObject, eventData.pointerClick.transform.parent.gameObject))
                            {
                                Debug.Log("Cant attack from position");
                                return;
                            }
                            if (cardObject.acted)
                            {
                                Debug.Log("Already attacked");
                                return;
                            }
                            GameStart.INSTANCE.GameState = GameStart.State.Battle;
                            GameObject enemyCardSlot = eventData.pointerClick.transform.parent.gameObject;
                            string enemyCardName = eventData.pointerClick.GetComponent<Image>().sprite.name;
                            int damage = playerCard.Attack + cardObject.counters;
                            bool destroysCard = GameStart.INSTANCE.Attack(enemyCardSlot, damage);
                            if (destroysCard)
                            {
                                EffectListener.INSTANCE.OnDestroyedCard(cardObject);
                                GameObject enemyCardGO = enemyCardSlot.transform.GetChild(3).gameObject;
                                GameStart.INSTANCE.CardGameObjectsInGame.Remove(enemyCardObject);
                                for (int i = 0; i < eventData.pointerClick.transform.childCount; i++)
                                {
                                    GameObject child = eventData.pointerClick.transform.GetChild(i).gameObject;
                                    if (child.name.StartsWith("CounterPanel"))
                                    {
                                        GameObject.Destroy(child);
                                    }
                                }
                                enemyCardGO.transform.SetParent(GameStart.INSTANCE.EnemyGraveyard.transform, false);
                                if (enemyCardName == GameStart.INSTANCE.EnemyLeader)
                                {
                                    GameStart.INSTANCE.Victory();
                                }
                                GameStart.INSTANCE.UpdateStatBoxes(cardObject, enemyCardSlot, enemyCardSlot);
                            }
                            Image cardImage = GameStart.INSTANCE.SelectedCardGameObject.GetComponent<UnityEngine.UI.Image>();
                            cardImage.color = new Color32(40, 40, 40, 255);
                            cardObject.acted = true;
                            RemovePreviousMark();
                            GameStart.INSTANCE.RecalculateCosts();
                            Debug.Log("Attack");
                        }
                    }
                    
                }
            }
            
        }
    }


    private static void UseEffect(GameObject eventData)
    {
        GameObject go = GameStart.INSTANCE.SelectedCardGameObject;
        CardObject cardObject = GameStart.INSTANCE.FindCardObject(go);
        GameStart.INSTANCE.UseCardEffect(cardObject, eventData);
        GameStart.INSTANCE.CardUsingEffect = null;
        RemovePreviousMark();
        Debug.Log("Used effect");
    }

    private bool CanMoveToPosition(GameObject movingCard, GameObject objectiveSlot)
    {
        GameObject originalSlot = movingCard.transform.parent.gameObject;

        int originalSlotY = Int32.Parse(originalSlot.name.Substring(8, 1));
        int originalSlotX = Int32.Parse(originalSlot.name.Substring(9, 1));
        int objectiveSlotY = Int32.Parse(objectiveSlot.name.Substring(8, 1));
        int objectiveSlotX = Int32.Parse(objectiveSlot.name.Substring(9, 1));

        if((originalSlotY == objectiveSlotY && originalSlotX - objectiveSlotX < 2 && (originalSlotX - objectiveSlotX > -2)) ||
           (originalSlotX == objectiveSlotX && (originalSlotY - objectiveSlotY < 2) && (originalSlotY - objectiveSlotY > -2)))
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    private bool IsAllyCard(GameObject card)
    {
        return card.name.EndsWith("Card") && (card.transform.parent.parent.name == "PlayerField" || card.transform.parent.name == "PlayerHandArea");
    }

    private bool IsEnemyCard(GameObject card)
    {
        return card.name.EndsWith("Card") && (card.transform.parent.parent.name == "EnemyField" || card.transform.parent.name == "EnemyHandArea");
    }

    private bool IsCardInHand(CardObject cardObject)
    {
        return cardObject != null && cardObject.GameObject.name.EndsWith("Card")  && (cardObject.GameObject.transform.parent.name == "PlayerHandArea" || cardObject.GameObject.transform.parent.name == "EnemyHandArea");
    }

    private bool EnemyCardInGame(GameObject card)
    {
        if (IsCard(card))
        {
            return card.transform.parent.parent.name == "EnemyField";
        }
        else
        {
            return false;
        }
    }

    private bool IsCard(GameObject card)
    {
        return card.name.EndsWith("Card");
    }

    private bool PlayerCardInGame(GameObject card)
    {
        if (IsCard(card))
        {
            return card.transform.parent.parent.name == "PlayerField";
        }
        else
        {
            return false;
        }
    }

    private bool CardInHand(GameObject card)
    {
        if (IsCard(card))
        {
            return card.transform.parent.name == "PlayerHandArea";
        }
        else
        {
            return false;
        }
    }

    private bool PlayerOpenSlot(GameObject card)
    {
        if (!IsCard(card))
        {
            return card.transform.parent.name == "PlayerField";
        }
        else
        {
            return false;
        }
    }

    public static void RemovePreviousMark()
    {
        if (GameStart.INSTANCE.SelectedCardGameObject != null)
        {
            GameObject mark = GameStart.INSTANCE.SelectedCardGameObject.transform.Find("SelectionMark").gameObject;
            Destroy(mark);
        }
        GameStart.INSTANCE.SelectedCardGameObject = null;
    }

    private void CreateMark()
    {
        RemovePreviousMark();
        Sprite sprite = (Sprite)Resources.Load("SelectionMark", typeof(Sprite));
        GameObject go = new GameObject("SelectionMark");
        RectTransform rectTransform = go.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(112, 160);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        go.transform.SetParent(this.transform, false);
        GameStart.INSTANCE.SelectedCardGameObject = go.transform.parent.gameObject;
    }

}
