using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static Deck;
using Image = UnityEngine.UI.Image;

public class CardFunctions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
        if((Game.INSTANCE.IsCard(this.gameObject) && this.transform.parent.name != "EnemyHandArea") || this.transform.parent.name == "PlayerHandArea")
        {
            CardObject co = Game.INSTANCE.FindCardObject(this.gameObject);
            if(co!=null && co.counters > 0)
            {
                GameObject cardCounterPanel = Instantiate(Game.INSTANCE.CountersPrefab, new Vector3(0, 0, 0), Quaternion.identity);
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
        if(Game.INSTANCE.GameState == Game.State.EnemyTurn)
        {
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            try
            {
                string cardName = this.GetComponent<Image>().sprite.name;
                CardObject cardObject = Game.INSTANCE.FindCardObject(this.gameObject);
                if (IsAllyCard(eventData.pointerClick))
                {
                    if (cardObject == null || (Game.INSTANCE.CardUsingEffect!=null && cardObject.card.PassiveEffect))
                    {
                        return;
                    }
                    if (CardInHand(cardObject.GameObject))
                    {
                        return;
                    }
                    if (cardObject.card.PassiveEffect)
                    {
                        Debug.Log("Can't manually activate passive effect");
                        return;
                    }
                    if (!cardObject.usedEffect && (Game.INSTANCE.GameState == Game.State.Moving || Game.INSTANCE.GameState == Game.State.Summoning))
                    {
                        if (!EffectListener.INSTANCE.OnAllowUseEffect(cardObject))
                        {
                            Debug.Log("A card is preventing you from activating this effect");
                            return;
                        }
                        if ((cardObject.card.Cooldown == 0 || cardObject.TurnEffectWasUsedOn < 0 || (cardObject.card.Cooldown + cardObject.TurnEffectWasUsedOn <= Game.INSTANCE.Turn)))
                        {
                            
                            if (!cardObject.card.UsesTarget)
                            {
                                Card c = cardObject.card;
                                c.InitializeEffectParametrs();
                                c.SetUsedByPlayer(true);
                                c.SetTargetCardObject(cardObject);
                                Game.INSTANCE.UseCardEffect(cardObject, null);
                                Debug.Log("Using effect");
                            }
                            else
                            {
                                CreateMark();
                                Game.INSTANCE.CardUsingEffect = cardObject.card;
                                Debug.Log("Using effect");
                            }
                        }
                        else
                        {
                            Debug.Log("This card needs to wait " + cardObject.card.Cooldown.ToString() + "+" + cardObject.TurnEffectWasUsedOn + "<=" + Game.INSTANCE.Turn);
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
            if (Game.INSTANCE.SelectedCardGameObject == null && IsAllyCard(eventData.pointerClick) && Game.INSTANCE.CardUsingEffect == null)
            {
                CreateMark();
                ShowPosibleMovements();
            }
            else
            {
                if (Game.INSTANCE.SelectedCardGameObject == null)
                {
                    return;
                }
                if (eventData.pointerClick.name == Game.INSTANCE.SelectedCardGameObject.name)
                {
                    if (Game.INSTANCE.CardUsingEffect != null)
                    {
                        UseEffect(eventData.pointerClick);
                    }
                    else
                    {
                        RemovePreviousMark();
                        Game.INSTANCE.HidePosibleMovements();
                        Game.INSTANCE.CardUsingEffect = null;
                    }
                }
                else
                {
                    if (Game.INSTANCE.CardUsingEffect != null)
                    {
                        CardObject co = Game.INSTANCE.FindCardObject(eventData.pointerClick);
                        if(co == null)
                        {
                            return;
                        }
                        TargetType effectType = co.card.TargetType;
                        TagType targetTag = Game.INSTANCE.CardUsingEffect.TargetTag;
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
                        if (CardInHand(Game.INSTANCE.SelectedCardGameObject) && PlayerOpenSlot(eventData.pointerClick) && (Game.INSTANCE.GameState == Game.State.Summoning || Game.INSTANCE.GameState == Game.State.Moving))
                        {
                            GameObject movingCard = Game.INSTANCE.SelectedCardGameObject;
                            string cardName = movingCard.GetComponent<Image>().sprite.name;
                            Card card = Game.INSTANCE.FindCard(cardName);
                            GameObject costBlock = eventData.pointerClick.transform.GetChild(2).GetChild(0).gameObject;
                            int costInSlot = Int32.Parse(costBlock.GetComponent<TextMeshProUGUI>().text);
                            if (card.Cost <= costInSlot)
                            {
                                Game.INSTANCE.PlayerHand.cards.Remove(card);
                                Game.INSTANCE.GameState = Game.State.Summoning;
                                GameObject previousParent = movingCard.transform.parent.gameObject;
                                movingCard.transform.SetParent(eventData.pointerClick.gameObject.transform, false);
                                movingCard.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                                CardObject co = new CardObject(movingCard, card);
                                Game.INSTANCE.CardObjectsInGame.Add(co);
                                EffectListener.INSTANCE.OnCardPlayed(co);
                                Game.INSTANCE.UpdateStatBoxes(co, eventData.pointerClick.gameObject, previousParent);
                                RemovePreviousMark();
                                Game.INSTANCE.HidePosibleMovements();
                                Game.INSTANCE.RecalculateCosts();
                                Game.INSTANCE.RearrangeHand(true);
                                Debug.Log("Put card in field");
                            }
                            else
                            {
                                Debug.Log("Cost too high for card");
                            }

                        }
                        else if (PlayerCardInGame(Game.INSTANCE.SelectedCardGameObject) && (PlayerOpenSlot(eventData.pointerClick) || PlayerCardInGame(eventData.pointerClick))&& Game.INSTANCE.GameState == Game.State.Moving)
                        {
                            GameObject movingCard = Game.INSTANCE.SelectedCardGameObject;
                            CardObject cardObject = Game.INSTANCE.FindCardObject(movingCard);
                            if (cardObject.moved)
                            {
                                Debug.Log("Already moved");
                                return;
                            }
                            if(!EffectListener.INSTANCE.OnAllowMovement(cardObject))
                            {
                                Debug.Log("Effect is stopping this card from moving");
                                return;
                            }
                            bool canMove = CanMoveToPosition(movingCard, eventData.pointerClick);
                            if (!canMove)
                            {
                                Debug.Log("Cant move to this spot");
                                return;
                            }
                            if (PlayerCardInGame(eventData.pointerClick))
                            {
                                CardObject steppedCO = Game.INSTANCE.FindCardObject(Game.INSTANCE.GetCardGameObject(eventData.pointerClick)); 
                                if(steppedCO.card.ImageName == Game.INSTANCE.PlayerLeader)
                                {
                                    Debug.Log("Can't step on leader card");
                                    return;
                                }
                                else
                                {
                                    Game.INSTANCE.CardObjectsInGame.Remove(steppedCO);
                                    steppedCO.GameObject.transform.SetParent(Game.INSTANCE.PlayerGraveyard.transform, false);
                                    CleanCard(steppedCO.GameObject);
                                }
                            }
                            cardObject.moved = true;
                            GameObject previousParent = movingCard.transform.parent.gameObject;
                            movingCard.transform.SetParent(eventData.pointerClick.gameObject.transform, false);
                            Game.INSTANCE.UpdateStatBoxes(cardObject, eventData.pointerClick.gameObject, previousParent);
                            RemovePreviousMark();
                            Game.INSTANCE.HidePosibleMovements();
                            Game.INSTANCE.RecalculateCosts();
                            Debug.Log("Card moves");
                        }
                        else if (PlayerCardInGame(Game.INSTANCE.SelectedCardGameObject) && EnemyCardInGame(eventData.pointerClick) && (Game.INSTANCE.GameState == Game.State.Summoning || Game.INSTANCE.GameState == Game.State.Moving || Game.INSTANCE.GameState == Game.State.Battle))
                        {
                            string playerCardName = Game.INSTANCE.SelectedCardGameObject.GetComponent<Image>().sprite.name;
                            Card playerCard = Game.INSTANCE.FindCard(playerCardName);
                            CardObject cardObject = Game.INSTANCE.FindCardObject(Game.INSTANCE.SelectedCardGameObject);
                            CardObject enemyCardObject = Game.INSTANCE.FindCardObject(eventData.pointerClick);
                            if (!Game.INSTANCE.CanAttack(Game.INSTANCE.SelectedCardGameObject.transform.parent.gameObject, eventData.pointerClick.transform.parent.gameObject))
                            {
                                Debug.Log("Cant attack from position");
                                return;
                            }
                            if (cardObject.acted)
                            {
                                Debug.Log("Already attacked");
                                return;
                            }
                            Game.INSTANCE.GameState = Game.State.Battle;
                            GameObject enemyCardSlot = eventData.pointerClick.transform.parent.gameObject;
                            string enemyCardName = eventData.pointerClick.GetComponent<Image>().sprite.name;
                            int damage = playerCard.Attack + cardObject.counters;
                            bool destroysCard = Game.INSTANCE.Attack(enemyCardSlot, damage);
                            if (destroysCard)
                            {
                                EffectListener.INSTANCE.OnDestroyedCard(cardObject);
                                GameObject enemyCardGO = Game.INSTANCE.GetCardGameObject(enemyCardSlot);
                                Game.INSTANCE.CardObjectsInGame.Remove(enemyCardObject);
                                CleanCard(enemyCardObject.GameObject);
                                enemyCardGO.transform.SetParent(Game.INSTANCE.EnemyGraveyard.transform, false);
                                if (enemyCardName == Game.INSTANCE.EnemyLeader)
                                {
                                    Game.INSTANCE.Victory();
                                }
                                Game.INSTANCE.UpdateStatBoxes(cardObject, enemyCardSlot, enemyCardSlot);
                            }
                            Image cardImage = Game.INSTANCE.SelectedCardGameObject.GetComponent<UnityEngine.UI.Image>();
                            cardImage.color = new Color32(40, 40, 40, 255);
                            cardObject.acted = true;
                            RemovePreviousMark();
                            Game.INSTANCE.HidePosibleMovements();
                            Game.INSTANCE.RecalculateCosts();
                            Debug.Log("Attack");
                        }
                    }
                    
                }
            }
            
        }
    }

    private static void CleanCard(GameObject go)
    {
        for (int i = 0; i < go.transform.childCount; i++)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            if (child.name.StartsWith("CounterPanel") || child.name == "Shield")
            {
                GameObject.Destroy(child);
            }
        }
    }

    private static void UseEffect(GameObject eventData)
    {
        GameObject go = Game.INSTANCE.SelectedCardGameObject;
        CardObject cardObject = Game.INSTANCE.FindCardObject(go);
        CardObject tagetCO = Game.INSTANCE.FindCardObject(eventData);
        Card c = cardObject.card;
        c.InitializeEffectParametrs();
        c.SetUsedByPlayer(true);
        Game.INSTANCE.UseCardEffect(cardObject, tagetCO);
        Game.INSTANCE.CardUsingEffect = null;
        RemovePreviousMark();
        Debug.Log("Used effect");
    }


    private void ShowPosibleMovements()
    {
        GameObject selectedCard = Game.INSTANCE.SelectedCardGameObject;
        if(selectedCard.transform.parent.name == Game.INSTANCE.PlayerHandArea.name)
        {
            return;
        }
        CardObject co = Game.INSTANCE.FindCardObject(selectedCard);
        if (co == null)
        {
            return;
        }
        if(co!=null && co.moved)
        {
            return;
        }
        for (int i=0; i<Game.INSTANCE.PlayerField.transform.childCount; i++)
        {
            GameObject slot = Game.INSTANCE.PlayerField.transform.GetChild(i).gameObject;
            bool canMove = CanMoveToPosition(selectedCard, slot);
            GameObject movementGuideGO = new GameObject("MovementGuide");
            RectTransform rectTransform = movementGuideGO.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(112, 160);
            Image movementGuideImage = movementGuideGO.AddComponent<Image>();
            Sprite guideMovementSprite = null;
            if (canMove && !Game.INSTANCE.SlotWithCard(slot))
            {
                //canMove
                guideMovementSprite = (Sprite)Resources.Load("Circle", typeof(Sprite));
            }
            else if(canMove && Game.INSTANCE.SlotWithCard(slot) && selectedCard.transform.parent.name != slot.name)
            {
                //can step on card
                guideMovementSprite = (Sprite)Resources.Load("Skull", typeof(Sprite));
            }
            else if(selectedCard.transform.parent.name != slot.name)
            {
                //cant move
                guideMovementSprite = (Sprite)Resources.Load("Cross", typeof(Sprite));
            }
            else
            {
                GameObject.Destroy(movementGuideGO);
            }
            movementGuideGO.transform.SetParent(slot.transform, false);
            movementGuideImage.sprite = guideMovementSprite;
        }
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
        if (Game.INSTANCE.IsCard(card))
        {
            return card.transform.parent.parent.name == "EnemyField";
        }
        else
        {
            return false;
        }
    }


    private bool PlayerCardInGame(GameObject card)
    {
        if (card.name.StartsWith("CardSlot"))
        {
            card = Game.INSTANCE.GetCardGameObject(card);
            if (card != null)
            {
                return card.transform.parent.parent.name == "PlayerField";
            }
            else
            {
                return false;
            }
        }
        if (Game.INSTANCE.IsCard(card))
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
        if (Game.INSTANCE.IsCard(card))
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
        if (card.name.StartsWith("CardSlot"))
        {
            card = Game.INSTANCE.GetCardGameObject(card);
            if (card != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        if (!Game.INSTANCE.IsCard(card))
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
        if (Game.INSTANCE.SelectedCardGameObject != null)
        {
            GameObject mark = Game.INSTANCE.SelectedCardGameObject.transform.Find("SelectionMark").gameObject;
            Destroy(mark);
        }
        Game.INSTANCE.SelectedCardGameObject = null;
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
        Game.INSTANCE.SelectedCardGameObject = go.transform.parent.gameObject;
    }

}
