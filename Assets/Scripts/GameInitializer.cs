using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameInitializer : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject EnemyPanel;
    [SerializeField] private TextMeshProUGUI EnemyName;
    [SerializeField] private TextMeshProUGUI VictoryText;
    [SerializeField] private TextMeshProUGUI DefeatText;
    [SerializeField] private TMP_InputField DeckNameTextInput;

    void Start()
    {
    }

    void Update()
    {
        int victories = PlayerPrefs.GetInt(EnemyName.text+"Victories");
        int defeats = PlayerPrefs.GetInt(EnemyName.text + "Defeats");
        VictoryText.text = victories.ToString();
        DefeatText.text = defeats.ToString();
    }

    private void ChangeScene()
    {
        string deckName = DeckNameTextInput.text;
        PlayerPrefs.SetString("EnemyName", EnemyName.text);
        PlayerPrefs.SetString("DeckName", deckName);
        SceneManager.LoadScene(1);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ChangeScene();
        }
    }
}
