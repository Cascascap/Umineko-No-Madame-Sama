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
        if(this.name.EndsWith("Card") || this.transform.parent.name == "PlayerHandArea")
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
                foreach (Deck d in GameStart.DecksInGame)
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
            Sprite sprite = (Sprite)Resources.Load("SelectionMark", typeof(Sprite));
            GameObject go = new GameObject("SelectionMark");
            RectTransform rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(112, 160);
            Image image = go.AddComponent<Image>();
            image.sprite = sprite;
            go.transform.SetParent(this.transform, false);
        }
    }

}
