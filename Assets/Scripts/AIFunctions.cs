using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AIFunctions : MonoBehaviour
{
    public static AIFunctions INSTANCE = null;
    public Hand hand;

    // Start is called before the first frame update
    void Start()
    {
        INSTANCE = this;
    }


    public void TakeTurn(Deck enemyDeck)
    {
        if (!IsLeaderOnTop())
        {
            MoveLeaderTop();
        }
        GameStart.INSTANCE.OnTurnStart();
    }

    private void MoveLeaderTop()
    {
        CardObject leaderCardObject = GameStart.INSTANCE.FindEnemyLeaderCardObject(GameStart.INSTANCE.EnemyLeader);
        GameObject leaderGameObject = leaderCardObject.GameObject;
        GameObject slot = leaderGameObject.transform.parent.gameObject;
        int locationY = Int32.Parse(slot.name.Substring(8, 1));
        int locationX = Int32.Parse(slot.name.Substring(9, 1));
        bool cardUp = GameStart.INSTANCE.CheckUp(locationX, locationY) == 1;
        if (!cardUp)
        {
            MoveCardUp(leaderCardObject, locationX, locationY, slot);
        }

    }

    private void MoveCardUp(CardObject card, int currentX, int currentY, GameObject previousSlot)
    {
        GameObject upSlot = GameStart.INSTANCE.GetSlotMap()[(currentY-1).ToString() + currentX.ToString()];
        GameObject newHPbox = upSlot.transform.GetChild(0).GetChild(0).gameObject;
        TextMeshProUGUI newHPText = newHPbox.GetComponent<TextMeshProUGUI>();
        newHPText.text = card.currentHP.ToString();
        GameObject newATKbox = upSlot.transform.GetChild(0).GetChild(0).gameObject;
        TextMeshProUGUI newATKText = newATKbox.GetComponent<TextMeshProUGUI>();
        newATKText.text = card.currentATK.ToString();

        card.GameObject.transform.SetParent(upSlot.transform, false);
        GameStart.INSTANCE.RecalculateCosts();
        GameStart.INSTANCE.UpdateStatBoxes(card, upSlot, previousSlot);
    }

    public bool IsLeaderOnTop()
    {
        List<CardObject> allCards= GameStart.INSTANCE.CardGameObjectsInGame;
        CardObject enemyLeader = allCards.Find(x => x.GameObject.name == GameStart.INSTANCE.EnemyLeader + "Card");
        string slotName = enemyLeader.GameObject.transform.parent.name;
        int yCoordinates = int.Parse(slotName.Substring(8, 1));
        return yCoordinates == 0;
    }
}
