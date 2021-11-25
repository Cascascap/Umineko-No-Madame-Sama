using System;
using System.Collections.Generic;
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
                Card clickedCard = null;
                foreach (Deck d in GameStart.INSTANCE.DecksInGame)
                {
                    Card c = d.FindCardInDeck(cardName);
                    if (c != null)
                    {
                        clickedCard = c;
                        break;
                    }
                }
                clickedCard.effect();
            }
            catch(UnassignedReferenceException)
            {
                Debug.Log("No card");
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (GameStart.INSTANCE.selectedCardGameObject == null && IsCard(eventData.pointerClick))
            {
                CreateMark();
            }
            else
            {
                if (GameStart.INSTANCE.selectedCardGameObject == null)
                {
                    return;
                }
                if (eventData.pointerClick.name == GameStart.INSTANCE.selectedCardGameObject.name)
                {
                    RemovePreviousMark();
                }
                else
                {
                    if(CardInHand(GameStart.INSTANCE.selectedCardGameObject) && PlayerOpenSlot(eventData.pointerClick))
                    {
                        GameObject movingCard = GameStart.INSTANCE.selectedCardGameObject;
                        movingCard.transform.SetParent(eventData.pointerClick.gameObject.transform, false);
                        movingCard.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                        RemovePreviousMark();
                        Debug.Log("Put card in field");
                    }
                    else if(PlayerCardInGame(GameStart.INSTANCE.selectedCardGameObject) && PlayerOpenSlot(eventData.pointerClick))
                    {
                        GameObject movingCard = GameStart.INSTANCE.selectedCardGameObject;
                        movingCard.transform.SetParent(eventData.pointerClick.gameObject.transform, false);
                        RemovePreviousMark();
                    }
                    else if (PlayerCardInGame(GameStart.INSTANCE.selectedCardGameObject) && EnemyCardInGame(eventData.pointerClick))
                    {
                        Debug.Log("Attack");
                    }
                }
            }
            
        }
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
        GameObject mark = GameStart.INSTANCE.selectedCardGameObject.transform.Find("SelectionMark").gameObject;
        Destroy(mark);
        GameStart.INSTANCE.selectedCardGameObject = null;
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
        GameStart.INSTANCE.selectedCardGameObject = go.transform.parent.gameObject;
    }
}
