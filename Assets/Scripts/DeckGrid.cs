using System;
using System.Collections;
using System.Collections.Generic;
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

    private void LoadCardsInInventory()
    {
        int i = 1;
        foreach(Deck.CardsByID cbi in Enum.GetValues(typeof(Deck.CardsByID)))
        {
            int cardsHold = PlayerPrefs.GetInt(cbi.ToString()+"Inventory");
            if(cardsHold != 0)
            {
                DeckGridTile cardTile = CardsMap[i];
                Sprite cardSprite = (Sprite)Resources.Load("Cards/" + cbi.ToString(), typeof(Sprite));
                cardTile.gameObject.GetComponent<Image>().sprite = cardSprite;
            }
            i++;
        }
    }

    void GoBackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
