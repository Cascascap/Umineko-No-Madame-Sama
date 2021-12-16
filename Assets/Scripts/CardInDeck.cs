using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardInDeck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI CardInDeckName;
    public TextMeshProUGUI CardInDeckCost;
    public TextMeshProUGUI CardInDeckHP;
    public TextMeshProUGUI CardInDeckATK;
    public Sprite CardInDeckSprite;
    public Image ZoomedCard;
    public Card Card;

    public void OnPointerClick(PointerEventData eventData)
    {
        CardInDeckManager.GetInstance().RemoveCardFromDeck(this);
        ZoomedCard.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ZoomedCard.gameObject.GetComponent<Image>().sprite = CardInDeckSprite;
        ZoomedCard.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ZoomedCard.gameObject.SetActive(false);
    }

    void Awake()
    {
        CardInDeckName = this.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        CardInDeckCost = this.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        CardInDeckHP = this.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
        CardInDeckATK = this.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();
    }

}
