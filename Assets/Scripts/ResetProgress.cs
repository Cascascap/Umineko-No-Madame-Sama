using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResetProgress : MonoBehaviour
{
    [SerializeField] private Button ResetButton;
    [SerializeField] private MainMenuManager MMM;
    // Start is called before the first frame update
    void Start()
    {
        ResetButton.onClick.AddListener(ResetAll);
    }

    private void ResetAll()
    {
        /*
         if (EditorUtility.DisplayDialog("Reset all progress",
                "Do you want to erease all your saved progress?", "Yes", "No"))
        {
            
        }
        else
        {
            Debug.Log("Not ereased");
        }
         */
        PlayerPrefs.DeleteAll();
        MMM.HideObjects();
        Debug.Log("Ereased");
    }
}
