using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CardZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IEndDragHandler
{
    public GameObject ZoomedCard;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Sprite sprite = this.GetComponent<Image>().sprite;
        ZoomedCard.GetComponent<Image>().sprite = sprite;
        ZoomedCard.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ZoomedCard.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            try
            {
                Card clickedCard = GameStart.cardList[this.GetComponent<Image>().sprite.name];
                clickedCard.effect();
            }
            catch(UnassignedReferenceException)
            {
                Debug.Log("No card");
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = GetMousePos(); 
        this.gameObject.transform.parent.transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        List<GameObject> hoveredItems = eventData.hovered;
        foreach(GameObject go in hoveredItems)
        {
            if(go.name == "Image")
            {
                Debug.Log("Hovering over " + go.transform.parent.name);
            }
            else
            {
                Debug.Log("Hovering over: " + go.name);
            }
        }
    }


    private Vector3 GetMousePos()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
}
