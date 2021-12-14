using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour, IPointerClickHandler
{
    public GameObject EnemyPanel;
    public string enemyName;
    public TextMeshProUGUI victoryText;
    public TextMeshProUGUI defeatText;

    void Start()
    {
        enemyName = EnemyPanel.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text;
        victoryText = EnemyPanel.transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>();
        defeatText = EnemyPanel.transform.GetChild(6).gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        int victories = PlayerPrefs.GetInt(enemyName+"Victories");
        int defeats = PlayerPrefs.GetInt(enemyName + "Defeats");
        victoryText.text = victories.ToString();
        defeatText.text = defeats.ToString();
    }

    private void ChangeScene()
    {
        PlayerPrefs.SetString("EnemyName", enemyName);
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
