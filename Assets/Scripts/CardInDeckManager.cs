using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInDeckManager : MonoBehaviour
{
    [SerializeField] private int Width, Height;
    [SerializeField] private CardInDeck CardInDeckPrefab;
    [SerializeField] private GameObject Background;
    // Start is called before the first frame update
    private int INITIALY = -25;
    private int OFFSET = -5;
    void Start()
    {
        int cardsInDeck = 40;
        for(int i=0; i<cardsInDeck; i++)
        {
            CardInDeck spawnedCard = Instantiate(CardInDeckPrefab, new Vector3(0, 0), Quaternion.identity, Background.transform);
            RectTransform rectTransform = spawnedCard.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector2(3, INITIALY + OFFSET - (Height*i));
            spawnedCard.name = "CardInDeck" + i.ToString();
        }

    }
}
