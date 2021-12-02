using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Deck;

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
        else if(!IsLeaderOnRight())
        {
            TakeSecondTurn();
        }
        else
        {
            TakeTurn();
        }
        UseEffects();
        AllAttack();
        GameStart.INSTANCE.RearrangeHand(false);
        GameStart.INSTANCE.OnTurnStart();
    }

    private void TakeTurn()
    {
        PlayBiggestCostPosibleFromHand();
    }

    private void PlayBiggestCostPosibleFromHand()
    {
        for (int i = 0; i < GameStart.INSTANCE.EnemyField.transform.childCount; i++)
        {
            GameObject slot = GameStart.INSTANCE.EnemyField.transform.GetChild(i).gameObject;
            if (!GameStart.INSTANCE.SlotWithCard(slot))
            {
                string slotCoordinates = slot.name.Substring(8, 2);
                int cost = Int32.Parse(slot.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text);
                Card succesfulSummon;
                succesfulSummon = PlayCardByCost(cost, slotCoordinates);
                if (succesfulSummon == null)
                {
                    succesfulSummon = PlayCardByCost(cost - 1, slotCoordinates);
                }
                else
                {
                    return;
                }
                if (succesfulSummon == null)
                {
                    succesfulSummon = PlayCardByCost(1, slotCoordinates);
                }
                else
                {
                    return;
                }
                if (succesfulSummon != null)
                {
                    return;
                }
            }
        }
    }


    private void TakeSecondTurn()
    {
        MoveLeaderRight();
        Card succesfulSummon;
        succesfulSummon = PlayCardByCost(2, "12");
        if (succesfulSummon == null)
        {
            PlayCardByCost(1, "12");
        }
        succesfulSummon = PlayCardByCost(3, "01");
        if (succesfulSummon == null)
        {
            succesfulSummon = PlayCardByCost(2, "01");
            if (succesfulSummon == null)
            {
                PlayCardByCost(1, "01");
            }
        }
    }

    private void MoveLeaderRight()
    {
        CardObject leaderCardObject = GameStart.INSTANCE.FindEnemyLeaderCardObject(GameStart.INSTANCE.EnemyLeader);
        GameObject leaderGameObject = leaderCardObject.GameObject;
        GameObject slot = leaderGameObject.transform.parent.gameObject;
        int locationY = Int32.Parse(slot.name.Substring(8, 1));
        int locationX = Int32.Parse(slot.name.Substring(9, 1));
        MoveCardRight(leaderCardObject, locationX, locationY, slot);
    }

    private void MoveCardRight(CardObject card, int locationX, int locationY, GameObject previousSlot)
    {
        GameObject rightSlot;
        try
        {
            rightSlot = GameStart.INSTANCE.GetSlotMap()[locationY.ToString() + (locationX + 1).ToString()];
        }
        catch(KeyNotFoundException)
        {
            return;
        }
        if (rightSlot!=null && SlotOccupied(rightSlot))
        {
            return;
        }
        GameObject newHPbox = rightSlot.transform.GetChild(0).GetChild(0).gameObject;
        TextMeshProUGUI newHPText = newHPbox.GetComponent<TextMeshProUGUI>();
        newHPText.text = card.currentHP.ToString();
        GameObject newATKbox = rightSlot.transform.GetChild(0).GetChild(0).gameObject;
        TextMeshProUGUI newATKText = newATKbox.GetComponent<TextMeshProUGUI>();
        newATKText.text = card.currentATK.ToString();

        card.GameObject.transform.SetParent(rightSlot.transform, false);
        GameStart.INSTANCE.RecalculateCosts();
        GameStart.INSTANCE.UpdateStatBoxes(card, rightSlot, previousSlot);
    }

    private void TakeFirstTurn()
    {
        MoveLeaderTop();
        Card succesfulSummon;
        PlayCardByCost(1, "11");
        PlayCardByCost(1, "00");
        succesfulSummon = PlayCardByCost(2, "10");
        if (succesfulSummon == null)
        {
            PlayCardByCost(1, "10");
        }
    }

    private void AllAttack()
    {
        for(int index = 0; index<GameStart.INSTANCE.CardGameObjectsInGame.Count; index++) 
        {
            CardObject co = GameStart.INSTANCE.CardGameObjectsInGame[index];
            if (co.IsEnemyCard())
            {
                CardObject bestTarget = GetBestTarget(co);
                if(bestTarget == null)
                {
                    continue;
                }
                GameObject targetCardSlot = bestTarget.GameObject.transform.parent.gameObject;
                bool destroysCard = GameStart.INSTANCE.Attack(targetCardSlot, co.card.Attack + co.counters);
                if (destroysCard)
                {
                    GameObject enemyCardGO = targetCardSlot.transform.GetChild(3).gameObject;
                    GameStart.INSTANCE.CardGameObjectsInGame.RemoveAt(index);
                    enemyCardGO.transform.SetParent(GameStart.INSTANCE.PlayerGraveyard.transform, false);
                    if (bestTarget.card.ImageName == GameStart.INSTANCE.PlayerLeader)
                    {
                        GameStart.INSTANCE.Defeat();
                    }
                    GameStart.INSTANCE.UpdateStatBoxes(bestTarget, null, previousParent: targetCardSlot);
                }
            }
        }
    }

    private CardObject GetBestTarget(CardObject co)
    {
        CardObject bestTarget = null;
        List<GameObject> attackOptions = GetAttackOptions(co);
        foreach(GameObject slot in attackOptions)
        {
            if (GameStart.INSTANCE.SlotWithCard(slot))
            {
                bool canAttack = GameStart.INSTANCE.CanAttack(co.GameObject.transform.parent.gameObject, slot);
                if (canAttack)
                {
                    CardObject candidate = GameStart.INSTANCE.FindCardObject(slot.transform.GetChild(3).gameObject);
                    if(candidate == null)
                    {
                        continue;
                    }
                    if (bestTarget == null)
                    {
                        bestTarget = candidate;
                    }
                    if(candidate.card.ImageName == GameStart.INSTANCE.PlayerLeader)
                    {
                        bestTarget = candidate;
                        return bestTarget;
                    }
                    if(bestTarget.currentHP > candidate.currentHP)
                    {
                        bestTarget = candidate;
                    }
                }
            }
        }
        return bestTarget;
    }

    private List<GameObject> GetAttackOptions(CardObject co)
    {
        List<GameObject> attackOptions = new List<GameObject>();
        GameObject cardSlot = co.GameObject.transform.parent.gameObject;
        for(int i = 2; i<= 3; i++)
        {
            for(int j=0; j<=2; j++)
            {
                bool canAttack = GameStart.INSTANCE.CanAttack(cardSlot, GameStart.INSTANCE.GetSlotMap()[i.ToString()+j.ToString()]);
                if (canAttack)
                {
                    attackOptions.Add(GameStart.INSTANCE.GetSlotMap()[i.ToString() + j.ToString()]);
                }
            }
        }
        return attackOptions;
    }

    private void UseEffects()
    {
        foreach (CardObject co in GameStart.INSTANCE.CardGameObjectsInGame) 
        {
            if (co.IsEnemyCard())
            {
                if (co.card.AutomaticEffect)
                {
                    GameStart.INSTANCE.UseCardEffect(co, null);
                }
                else
                {
                    TagType targetTag = co.card.TargetTag;
                    CardObject target = GetCardInFieldByTag(targetTag);
                    if(target == null)
                    {
                        return;
                    }
                    GameStart.INSTANCE.UseCardEffect(co, target.GameObject);
                }
            }
        }
    }

    private CardObject GetCardInFieldByTag(TagType targetTag)
    {
        foreach (CardObject co in GameStart.INSTANCE.CardGameObjectsInGame)
        {
            if (co.card.tags.Contains(targetTag))
            {
                return co;
            }
        }
        return null;
    }

    private Card PlayCardByCost(int cost, string slotNumber)
    {
        Card card = GetCardByCostFromHand(cost);
        if (card != null)
        {
            return PlayCardInSlot(card, slotNumber);
        }
        return null;
    }

    private Card PlayCardInSlot(Card card, string slotNumber)
    {
        GameObject slot = GameObject.Find("CardSlot" + slotNumber);
        if (SlotOccupied(slot))
        {
            return null;
        }
        GameObject slotBox = slot.transform.GetChild(2).GetChild(0).gameObject;
        TextMeshProUGUI slotText = slotBox.GetComponent<TextMeshProUGUI>();
        if(card.Cost < Int32.Parse(slotText.text))
        {
            return null;
        }
        CardObject co = GameStart.INSTANCE.PlayCardInSlot(card.ImageName, slot);
        GameStart.INSTANCE.CardGameObjectsInGame.Add(co);
        return co.card;
    }

    private Card GetCardByCostFromHand(int cost)
    {
        for (int i = 0; i< GameStart.INSTANCE.EnemyHandArea.transform.childCount; i++)
        {
            GameObject cardInHand = GameStart.INSTANCE.EnemyHandArea.transform.GetChild(i).gameObject;
            string cardName = cardInHand.GetComponent<Image>().sprite.name;
            Card c = GameStart.INSTANCE.FindCard(cardName);
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
        MoveCardUp(leaderCardObject, locationX, locationY, slot);

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

    private bool IsLeaderOnRight()
    {
        List<CardObject> allCards = GameStart.INSTANCE.CardGameObjectsInGame;
        CardObject enemyLeader = allCards.Find(x => x.GameObject.name == GameStart.INSTANCE.EnemyLeader + "Card");
        string slotName = enemyLeader.GameObject.transform.parent.name;
        int xCoordinates = int.Parse(slotName.Substring(9, 1));
        return xCoordinates == 2;
    }

}
