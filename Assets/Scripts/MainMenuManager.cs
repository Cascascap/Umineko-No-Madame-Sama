using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject Image;
    [SerializeField] private Button DeckButton;
    [SerializeField] private GameObject DeckName;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("PlayerHasDeck") == 1)
        {
            Image.SetActive(true);
            DeckButton.gameObject.SetActive(true);
            DeckName.SetActive(true);
        }
    }

    public void HideObjects()
    {
        Image.SetActive(false);
        DeckButton.gameObject.SetActive(false);
        DeckName.SetActive(false);
    }

}
