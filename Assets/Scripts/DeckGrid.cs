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
    private int OFFSET = 20;
    private int STARTINGX = -250;
    private int STARTINGY = 800;

    private void Start()
    {
        GenerateGrid();
        GoBackButton.onClick.AddListener(GoBackToMainMenu);
    }

    void GenerateGrid()
    {
        for (int y = Height; y > 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                var spawnedTile = Instantiate(TilePrefab, new Vector3((((x * 100) + (OFFSET*x)) + STARTINGX), (((y * 140) + (OFFSET*y)) - STARTINGY )), Quaternion.identity);
                spawnedTile.name = $"Tile{x}-{y}";
                spawnedTile.transform.SetParent(Background.transform, false);
            }
        }
    }

    void GoBackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
