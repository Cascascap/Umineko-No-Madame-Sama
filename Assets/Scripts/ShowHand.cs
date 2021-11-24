using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class ShowHand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject PlayerHandArea;

    void Start()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 position = PlayerHandArea.transform.position;
        PlayerHandArea.transform.position = new Vector3(position.x, position.y + 25);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Vector3 position = PlayerHandArea.transform.position;
        PlayerHandArea.transform.position = new Vector3(position.x, position.y - 25);
    }
}
