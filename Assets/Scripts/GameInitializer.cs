using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameInitializer : MonoBehaviour, IPointerClickHandler
{
    public GameObject EnemyPanel;
    public TextMeshProUGUI EnemyName;
    public TextMeshProUGUI VictoryText;
    public TextMeshProUGUI DefeatText;

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
        PlayerPrefs.SetString("EnemyName", EnemyName.text);
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
