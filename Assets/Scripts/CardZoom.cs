using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CardZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject ZoomedCard;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //If theres a card in the hovered slot or its a card in hand
        if(IsCard(this.gameObject) || this.transform.parent.name == "PlayerHandArea")
        {
            Sprite sprite = this.GetComponent<Image>().sprite;
            ZoomedCard.GetComponent<Image>().sprite = sprite;
            ZoomedCard.SetActive(true);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
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
                if (!cardObject.usedEffect && (GameStart.INSTANCE.GameState == GameStart.State.Moving || GameStart.INSTANCE.GameState == GameStart.State.Summoning))
                {
                    UseCardEffect(cardName);
                    cardObject.usedEffect = true;
                    Debug.Log("Using effect");
                }
                else
                {
                    Debug.Log("Already used this effect");
                }
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No card");
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (GameStart.INSTANCE.SelectedCardGameObject == null && IsAllyCard(eventData.pointerClick))
            {
                CreateMark();
            }
            else if (IsAllyCard(eventData.pointerClick))
            {
                RemovePreviousMark(); 
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
                }
                else
                {
                    if(CardInHand(GameStart.INSTANCE.SelectedCardGameObject) && PlayerOpenSlot(eventData.pointerClick) && (GameStart.INSTANCE.GameState == GameStart.State.Summoning || GameStart.INSTANCE.GameState == GameStart.State.Moving))
                    {
                        GameStart.INSTANCE.GameState = GameStart.State.Summoning;
                        GameObject movingCard = GameStart.INSTANCE.SelectedCardGameObject;
                        string cardName = movingCard.GetComponent<Image>().sprite.name;
                        Card card = GameStart.INSTANCE.FindCard(cardName);
                        GameObject costBlock = eventData.pointerClick.transform.GetChild(2).GetChild(0).gameObject;
                        int costInSlot = Int32.Parse(costBlock.GetComponent<TextMeshProUGUI>().text);
                        if (card.cost <= costInSlot)
                        {
                            GameObject previousParent = movingCard.transform.parent.gameObject;
                            movingCard.transform.SetParent(eventData.pointerClick.gameObject.transform, false);
                            movingCard.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                            GameStart.INSTANCE.CardGameObjectsInGame.Add(new CardObject(movingCard));
                            GameStart.INSTANCE.UpdateStatBoxes(card, eventData.pointerClick.gameObject, previousParent);
                            RemovePreviousMark();
                            GameStart.INSTANCE.RecalculateCosts();
                            Debug.Log("Put card in field");
                        }
                        else
                        {
                            Debug.Log("Cost too high for card");
                        }
                        
                    }
                    else if(PlayerCardInGame(GameStart.INSTANCE.SelectedCardGameObject) && PlayerOpenSlot(eventData.pointerClick) && GameStart.INSTANCE.GameState == GameStart.State.Moving)
                    {
                        GameObject movingCard = GameStart.INSTANCE.SelectedCardGameObject;
                        CardObject cardObject = GameStart.INSTANCE.FindCardObject(movingCard);
                        if(cardObject.moved)
                        {
                            Debug.Log("Already moved");
                            return;
                        }
                        cardObject.moved = true;
                        GameObject previousParent = movingCard.transform.parent.gameObject;
                        string cardName = movingCard.GetComponent<Image>().sprite.name;
                        movingCard.transform.SetParent(eventData.pointerClick.gameObject.transform, false);
                        Card card = GameStart.INSTANCE.FindCard(cardName);
                        GameStart.INSTANCE.UpdateStatBoxes(card, eventData.pointerClick.gameObject, previousParent);
                        RemovePreviousMark();
                        GameStart.INSTANCE.RecalculateCosts();
                        Debug.Log("Card moves");
                    }
                    else if (PlayerCardInGame(GameStart.INSTANCE.SelectedCardGameObject) && EnemyCardInGame(eventData.pointerClick) && (GameStart.INSTANCE.GameState == GameStart.State.Summoning || GameStart.INSTANCE.GameState == GameStart.State.Moving || GameStart.INSTANCE.GameState == GameStart.State.Battle))
                    {
                        string playerCardName = GameStart.INSTANCE.SelectedCardGameObject.GetComponent<Image>().sprite.name;
                        Card playerCard = GameStart.INSTANCE.FindCard(playerCardName);
                        CardObject cardObject = GameStart.INSTANCE.FindCardObject(GameStart.INSTANCE.SelectedCardGameObject);
                        if (cardObject.acted)
                        {
                            Debug.Log("Already attacked");
                            return; 
                        }
                        GameStart.INSTANCE.GameState = GameStart.State.Battle;
                        GameObject enemyCardSlot = eventData.pointerClick.transform.parent.gameObject;
                        string enemyCardName = eventData.pointerClick.GetComponent<Image>().sprite.name;
                        bool destroysCard = GameStart.INSTANCE.Attack(enemyCardSlot, playerCard.attack);
                        if (destroysCard)
                        {
                            GameObject enemyCardGO = enemyCardSlot.transform.GetChild(3).gameObject;
                            GameStart.INSTANCE.CardGameObjectsInGame.Remove(cardObject);
                            GameObject.Destroy(enemyCardGO);
                            if(enemyCardName == GameStart.INSTANCE.EnemyLeader)
                            {
                                GameStart.INSTANCE.Victory();
                            }
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

    private static void UseCardEffect(string cardName)
    {
        Card card = GameStart.INSTANCE.FindCard(cardName);
        card.effect();
    }

    private bool IsAllyCard(GameObject card)
    {
        return card.name.EndsWith("Card") && (card.transform.parent.parent.name == "PlayerField" || card.transform.parent.name == "PlayerHandArea");
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

    private void RemovePreviousMark()
    {
        GameObject mark = GameStart.INSTANCE.SelectedCardGameObject.transform.Find("SelectionMark").gameObject;
        Destroy(mark);
        GameStart.INSTANCE.SelectedCardGameObject = null;
    }

    private void CreateMark()
    {
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
