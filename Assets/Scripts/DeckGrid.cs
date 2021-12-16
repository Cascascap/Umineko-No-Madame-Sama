using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckGrid : MonoBehaviour
{
    [SerializeField] private int Width, Height;
    [SerializeField] private DeckGridTile TilePrefab;
    [SerializeField] private GameObject Background;
    [SerializeField] private Button GoBackButton;
    [SerializeField] private Image ZoomedCardInInventory;
    [SerializeField] private Image ZoomedCardInDeck;
    [SerializeField] private TextMeshProUGUI CardsOwnedText;
    private int OFFSET = 20;
    private int STARTINGX = -250;
    private int STARTINGY = 800;

    private Dictionary<int, DeckGridTile> CardsMap = new Dictionary<int, DeckGridTile>();

    private void Start()
    {
        GenerateGrid();
        GoBackButton.onClick.AddListener(GoBackToMainMenu);
    }

    void GenerateGrid()
    {
        int i = 1;
        for (int y = Height; y > 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                DeckGridTile spawnedTile = Instantiate(TilePrefab, new Vector3((((x * 100) + (OFFSET*x)) + STARTINGX), (((y * 140) + (OFFSET*y)) - STARTINGY )), Quaternion.identity);
                spawnedTile.name = $"Tile-{i}";
                spawnedTile.transform.SetParent(Background.transform, false);
                CardsMap.Add(i, spawnedTile);
                spawnedTile.ZoomedCard = ZoomedCardInInventory;
                i++;
            }
        }
        LoadCardsInInventory();
    }

    public void LoadCardsInInventory()
    {
        int i = 1;
        int cardsOwned = 0;
        foreach(Deck.CardsByID cbi in Enum.GetValues(typeof(Deck.CardsByID)))
        {
            int cardsHold = PlayerPrefs.GetInt(cbi.ToString()+"Inventory");
            DeckGridTile cardTile = CardsMap[i];
            Image tileImage = cardTile.gameObject.GetComponent<Image>();
            if (cardsHold > 0)
            {
                Sprite cardSprite = (Sprite)Resources.Load("Cards/" + cbi.ToString(), typeof(Sprite));
                tileImage.sprite = cardSprite;
                tileImage.color = new Color32(255, 255, 255, 255);
                cardTile.Card = Deck.GetAllCards().Find(x => x.ImageName == cbi.ToString());
;               cardsOwned++;
            }
            else
            {
                if(PlayerPrefs.GetInt(cbi.ToString())>0)
                {
                    Sprite cardSprite = (Sprite)Resources.Load("Cards/" + cbi.ToString(), typeof(Sprite));
                    tileImage.sprite = cardSprite;
                    cardTile.Card = Deck.GetAllCards().Find(x => x.ImageName == cbi.ToString());
                    tileImage.color = new Color32(40, 40, 40, 255);
                }
                else
                {
                    cardTile.gameObject.SetActive(false);
                }
            }
            i++;
        }
        CardsOwnedText.text = $"{cardsOwned}/45";
    }

    void GoBackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
