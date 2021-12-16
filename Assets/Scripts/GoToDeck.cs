using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToDeck : MonoBehaviour
{
    [SerializeField] private Button DeckButton;
    // Start is called before the first frame update
    void Start()
    {
        DeckButton.onClick.AddListener(GoToDeckBuilding);
    }

    private void GoToDeckBuilding()
    {
        SceneManager.LoadScene(2);
    }

}
