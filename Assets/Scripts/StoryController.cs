using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image SceneImage;
    [SerializeField] private Image Speaker1Image;
    [SerializeField] private Image Speaker2Image;
    [SerializeField] private GameObject Textbox;
    private int SceneState = 1;
    private int LastState = 0;
    private string SceneToPlay;


    void Start()
    {
        //SceneToPlay = PlayerPrefs.GetString("SceneToPlay");
        SceneToPlay = "Tutorial";
    }

    private void Tutorial()
    {
        switch (SceneState)
        {
            case 1:
                Speak(1, "Leaders/Beatrice2", "Leaders/Lambda2");
                ShowText("Allow me to explain how the game works");
                ShowImage("TutorialImages/Tutorial1");
                break;
            case 2:
                Speak(2, "Leaders/Beatrice2", "Leaders/Lambda2");
                ShowText("Omegalul");
                ShowImage("TutorialImages/Tutorial2");
                break;
        }
    }

    void Update()
    {
        if(LastState == SceneState)
        {
            return;
        }
        if (SceneToPlay == "Tutorial")
        {
            Tutorial();
        }
    }

    private void Speak(int speaker, string imageNameSpeaker1, string imageNameSpeaker2)
    {
        Image speakerImage;
        Image otherSpeaker;
        if(speaker == 1)
        {
            speakerImage = Speaker1Image;
            otherSpeaker = Speaker2Image;
        }
        else
        {
            speakerImage = Speaker2Image;
            otherSpeaker = Speaker1Image;
        }
        Speaker1Image.gameObject.SetActive(true);
        Sprite imageSprite = (Sprite)Resources.Load(imageNameSpeaker1, typeof(Sprite));
        Speaker1Image.sprite = imageSprite;

        Speaker2Image.gameObject.SetActive(true);
        Sprite imageSprite2 = (Sprite)Resources.Load(imageNameSpeaker2, typeof(Sprite));
        Speaker2Image.sprite = imageSprite2;


        speakerImage.color = new Color32(255, 255, 255, 255);
        otherSpeaker.color = new Color32(255, 255, 255, 100);
    }

    private void ShowImage(string spriteName)
    {
        SceneImage.gameObject.SetActive(true);
        Sprite imageSprite = (Sprite)Resources.Load(spriteName, typeof(Sprite));
        SceneImage.sprite = imageSprite;
    }

    private void ShowText(string text)
    {
        Textbox.SetActive(true);
        TextMeshProUGUI TextboxText = Textbox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextboxText.text = text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LastState = SceneState;
        SceneState ++;
    }
}
