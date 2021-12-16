using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckGridTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] public Image ZoomedCard;

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Image tileImage = this.gameObject.GetComponent<Image>();
        ZoomedCard.gameObject.GetComponent<Image>().sprite = tileImage.sprite;
        ZoomedCard.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ZoomedCard.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
