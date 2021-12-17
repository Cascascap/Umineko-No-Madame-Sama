using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image SceneImage;
    [SerializeField] private Image Speaker1Image;
    [SerializeField] private Image Speaker2Image;
    [SerializeField] private GameObject Textbox;
    private int SceneState;
    private int LastState;
    private string SceneToPlay;


    void Start()
    {
        SceneState = 1;
        LastState = 0;
        SceneToPlay = PlayerPrefs.GetString("SceneToPlay");
    }

    private void Story0()
    {
        switch (SceneState)
        {
            case 1:
                break;

        }
    }

    private void Tutorial()
    {
        switch (SceneState)
        {
            case 1:
                Speak(2, "Leaders/Beatrice2", "Leaders/Lambda2");
                ShowText("Hey there. Allow me to explain how the game works");
                break;
            case 2:
                ShowText("This is a cards game, you and your opponent will take turns playing cards and trying to destroy eachother. Each player is represented in the field by a card, if you can make your opponent's card health drop to 0, you will win the game");
                break;
            case 3:
                ShowImage("TutorialImages/Tutorial1");
                ShowText("The card attack is what a card will use to lower the enemy's card LP. When a card's LP becomes 0 the card is destroyed");
                break;
            case 4:
                ShowText("If your leader's LP becomes 0, you will lose the game");
                break;
            case 5:
                ShowImage("TutorialImages/Tutorial2");
                ShowText("You can zoom on cards to see their details, but know that the zoomed version will only show the card's original values, if a card loses LP, it wont be reflected in the zoomed version");
                break;
            case 6:
                ShowText("A card's cost indicates in which slot it can be played. If a slot has a cost value equal or higher to the cost in the card, you can summon that card in that slot");
                break;
            case 7:
                ShowText("Leader cards have a cost of 0, because they start in the field");
                break;
            case 8:
                ShowText("Tags categorize the cards, some effects can only be used on cards with certain tags");
                break;
            case 9:
                ShowImage("TutorialImages/Tutorial3");
                ShowText("To interact with a card left click over it. A red marker will appear on it. You can only interact with 1 card at a time, so make sure to unmark it if you want to interact with another card");
                break;
            case 10:
                ShowText("While on Moving phase you can move your cards to other empty slots");
                break;
            case 11:
                ShowText("If you want to move a card, select it and select an empty spot adjacent to it");
                break;
            case 12:
                ShowText("A card can only move Up, Down, Left or Right and only once per turn");
                break;
            case 13:
                ShowImage("TutorialImages/Tutorial4");
                ShowText("Notice how the cost in the slots change when you move your card");
                break;
            case 14:
                ShowText("A slot cost value is increased by 1 for every adjacent ally card. Since these 3 slots are connected to your card, they all have a cost value of 1");
                break;
            case 15:
                ShowText("If a slost cost is 0, you can't summon anything in it");
                break;
            case 16:
                ShowImage("TutorialImages/Tutorial5");
                ShowText("Once you start summoning cards, the Turn Phase will change to Summoning");
                break;
            case 17:
                ShowText("While on Summoning phase you can't move your cards in the field any longer until your next turn");
                break;
            case 18:
                ShowText("Notice again how the cost in the spots now adjacent to the summoned card had their costs recalculated");
                break;
            case 19:
                ShowImage("TutorialImages/Tutorial6");
                ShowText("Lets try summoning a card with Cost 2. By summoning another goat this empty slot now has 2 adjacent cards to it, and its cost value is now 2");
                break;
            case 20:
                ShowImage("TutorialImages/Tutorial7");
                ShowText("We can now Summon Lucifer who has a cost of 2 in the empty slot");
                break;
            case 21:
                ShowImage("TutorialImages/Tutorial8");
                ShowText("In the card detail you can also see a card's effect. Not all card have effects however, for example goats don't do anything special in the game");
                break;
            case 22:
                ShowText("To use a card effect right click the card. If the effect has no target, then it will automatically activate. Beatrice's effect is drawing a random Stake from the deck");
                break;
            case 23:
                ShowImage("TutorialImages/Tutorial9");
                ShowText("In this case, it seems we drew Asmodeus, lets summon her and use her effect too");
                break;
            case 24:
                ShowImage("TutorialImages/Tutorial10");
                ShowText("Some effects require you to target another card, like in Asmodeus' case");
                break;
            case 25:
                ShowText("Some card effects can only target specific cards, for example Asmodeus can only target 'Summoned' cards");
                break;
            case 26:
                ShowText("To use her effect Right click the card and a marker will appear on it. Now simply left click the target card");
                break;
            case 27:
                ShowImage("TutorialImages/Tutorial11");
                ShowText("Asmodeus' effect grants a card a +1/+1 counter. This means the card will gain 1 ATK and 1 LP. Lets try attacking with it now");
                break;
            case 28:
                ShowImage("TutorialImages/Tutorial12");
                ShowText("The card ATK is now 3, so we damaged Lambda by 3 LP and her health is now down to 33");
                break;
            case 29:
                ShowText("After a card attacks it will turn dark and wont be able to be used until the next turn");
                break;
            case 30:
                ShowText("After attacking you will move to Battle phase, and wont be able to use effects any longer");
                break;
            case 31:
                ShowText("A card can only attack an enemy card its in contact with, for example the middle goat could attack any enemy card");
                break;
            case 32:
                ShowText("A card cant attack if it has ally cards in front of it, so a card in the back row wont be able to attack in this situation");
                break;
            case 33:
                ShowText("Once we attack with all of our cards, we will click on the End Turn button and let the enemy have its turn");
                break;
            case 34:
                ShowImage("TutorialImages/Tutorial13");
                ShowText("The enemy will now take its turn, summoning cards, using its effects and attacking");
                break;
            case 35:
                ShowText("If an enemy card destroys one of your cards, your card will be moved to the cemetery and out of the game");
                break;
            case 36:
                ShowText("Once the enemy's turn is over, the player turn will start once again and the Turn Phase will start in Moving");
                break;
            case 37:
                ShowImage("TutorialImages/Tutorial14");
                ShowText("You can move over non ally cards, this will appear as a skull over the card that will be stepped on");
                break;
            case 38:
                ShowImage("TutorialImages/Tutorial15");
                ShowText("If a card is stepped on, the card will be destroyed and sent to the cemetery");
                break;
            case 39:
                ShowText("if there's something you wish to undo, you can restart your turn and do it again");
                break;
            case 40:
                ShowImage("TutorialImages/Tutorial16");
                ShowText("Cards destroyed, effects and attacks will all reset to their state at the start of the turn");
                break;
            case 41:
                ShowImage("TutorialImages/Tutorial17");
                ShowText("The game will proceed until one of the leaders LP goes down to 0");
                break;
            case 42:
                ShowImage("TutorialImages/Tutorial18");
                ShowText("When you beat the enemy, you will gain one of their cards to add to your deck. Press Main Menu and continue the game");
                break;
            case 43:
                //PlayerPrefs.SetInt(ProgressFlagsEnum.Flags.tutorial.ToString(), 1);
                BackToMainMenu();
                break;
        }
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }


    void Update()
    {
        if(LastState == SceneState)
        {
            return;
        }
        else if (SceneToPlay == "Tutorial")
        {
            Tutorial();
        }
        else if (SceneToPlay == "Story0")
        {
            Story0();
        }
    }

    private void Speak(int speaker, string imageNameSpeaker1, string imageNameSpeaker2, bool showSpeaker1= false, bool showSpeaker2=false)
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
        Speaker1Image.gameObject.SetActive(showSpeaker1);
        Speaker2Image.gameObject.SetActive(showSpeaker2);

        Sprite imageSprite = (Sprite)Resources.Load(imageNameSpeaker1, typeof(Sprite));
        Speaker1Image.sprite = imageSprite;

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
