using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class FieldCardZoom  : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject ZoomedCard;
    public GameObject ThisCard;

    void Start()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Sprite sprite = ThisCard.transform.Find("Image").GetComponent<Image>().sprite;
        ZoomedCard.GetComponent<Image>().sprite = sprite;
        ZoomedCard.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ZoomedCard.SetActive(false);
    }
}

