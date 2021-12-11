using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour, IPointerClickHandler
{
    public GameObject EnemyPanel;
    void Start()
    {
        
    }

    private void ChangeScene()
    {
        string enemyName = EnemyPanel.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text;
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
