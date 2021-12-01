using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AIFunctions : MonoBehaviour
{
    public static AIFunctions INSTANCE = null;

    // Start is called before the first frame update
    void Start()
    {
        INSTANCE = this;
        
    }


    public void TakeTurn(Deck enemyDeck)
    {
        GameStart.INSTANCE.Draw(GameStart.INSTANCE.EnemyDeck, 1);
        GameStart.INSTANCE.RearrangeHand(false);
        if (!IsLeaderOnTop())
        {
            TakeFirstTurn();
        }
        GameStart.INSTANCE.OnTurnStart();
    }

    private void TakeFirstTurn()
    {
        MoveLeaderTop();
        bool succesfulSummon;
        PlayCardByCost(1, "11");
        PlayCardByCost(1, "00");
        succesfulSummon = PlayCardByCost(2, "10");
        if (!succesfulSummon)
        {
            PlayCardByCost(1, "10");
        }
        UseEffects();
        Attack();
        GameStart.INSTANCE.RearrangeHand(false);
    }

    private void Attack()
    {
        throw new NotImplementedException();
    }

    private void UseEffects()
    {
        throw new NotImplementedException();
    }

    private bool PlayCardByCost(int cost, string slotNumber)
    {
        Card card = GetCardByCostFromHand(cost);
        if (card != null)
        {
            return PlayCardInSlot(card, slotNumber);
        }
        return false;
    }

    private bool PlayCardInSlot(Card card, string slotNumber)
    {
        GameObject slot = GameObject.Find("CardSlot" + slotNumber);
        if (SlotOccupied(slot))
        {
            return false;
        }
        GameObject slotBox = slot.transform.GetChild(2).GetChild(0).gameObject;
        TextMeshProUGUI slotText = slotBox.GetComponent<TextMeshProUGUI>();
        if(card.Cost < Int32.Parse(slotText.text))
        {
            return false;
        }
        CardObject co = GameStart.INSTANCE.CreateCardInSlot(card.ImageName, slot);
        GameStart.INSTANCE.CardGameObjectsInGame.Add(co);
        return true;
    }

    private Card GetCardByCostFromHand(int cost)
    {
        foreach (Card c in GameStart.INSTANCE.EnemyHand.cards)
        {
            if (c.Cost == cost)
            {
                return c;
            }
        }
        return null;
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
        if (SlotOccupied(upSlot))
        {
            return;
        }
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


    private bool SlotOccupied(GameObject go)
    {
        return go.transform.childCount > 3;
    }

    private bool IsLeaderOnTop()
    {
        List<CardObject> allCards= GameStart.INSTANCE.CardGameObjectsInGame;
        CardObject enemyLeader = allCards.Find(x => x.GameObject.name == GameStart.INSTANCE.EnemyLeader + "Card");
        string slotName = enemyLeader.GameObject.transform.parent.name;
        int yCoordinates = int.Parse(slotName.Substring(8, 1));
        return yCoordinates == 0;
    }

}
