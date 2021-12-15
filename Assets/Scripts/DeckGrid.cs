using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckGrid : MonoBehaviour
{
    [SerializeField] private int Width, Height;
    [SerializeField] private DeckGridTile TilePrefab;
    [SerializeField] private GameObject Background;
    [SerializeField] private Transform Camera;
    private int OFFSET = 20;
    private int STARTINGX = -320;
    private int STARTINGY = 940;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int y = Height; y > 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                var spawnedTile = Instantiate(TilePrefab, new Vector3((((x * 100) + (OFFSET*x)) + STARTINGX), (((y * 140) + (OFFSET*y)) - STARTINGY )), Quaternion.identity);
                //RectTransform rectTransform = spawnedTile.GetComponent<RectTransform>();
                //rectTransform.sizeDelta = new Vector2(100, 140);
                spawnedTile.name = $"Tile{x}-{y}";
                spawnedTile.transform.SetParent(Background.transform, false);
            }
        }
        
       // Camera.transform.position = new Vector3((float)Width / 2 - 0.5f, (float)Height / 2 - 0.5f, -10);
    }
}
