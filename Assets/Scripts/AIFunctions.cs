using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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


    public async void TakeTurn(Deck enemyDeck)
    {
        Game.INSTANCE.Draw(Game.INSTANCE.EnemyDeck, 1);
        Game.INSTANCE.RearrangeHand(false);
        await KinzoAI();
        await UseEffects();
        await AllAttack();
        Game.INSTANCE.RearrangeHand(false);
        Game.INSTANCE.OnTurnStart();
    }

    private async Task KinzoAI()
    {
        if (!IsLeaderOnTop())
        {
            await TakeFirstTurn();
        }
        else if (!IsLeaderOnRight())
        {
            await TakeSecondTurn();
        }
        else
        {
            await TakeTurn();
        }
    }

    private async void LambdaAI()
    {
        if (!IsLeaderOnTop())
        {
            await TakeFirstTurn();
        }
        else if (!IsLeaderOnRight())
        {
            await TakeSecondTurn();
        }
        else
        {
            await TakeTurn();
        }
    }

    private async Task TakeTurn()
    {
        await PlayBiggestCostPosibleFromHand();
    }

    private async Task PlayBiggestCostPosibleFromHand()
    {
        for (int i = 0; i < Game.INSTANCE.EnemyField.transform.childCount; i++)
        {
            GameObject slot = Game.INSTANCE.EnemyField.transform.GetChild(i).gameObject;
            if (!Game.INSTANCE.SlotWithCard(slot))
            {
                string slotCoordinates = slot.name.Substring(8, 2);
                int cost = Int32.Parse(slot.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text);
                Card succesfulSummon;
                succesfulSummon = await PlayCardByCost(cost, slotCoordinates);
                if (succesfulSummon == null)
                {
                    succesfulSummon = await PlayCardByCost(cost - 1, slotCoordinates);
                }
                else
                {
                    return;
                }
                if (succesfulSummon == null)
                {
                    succesfulSummon = await PlayCardByCost(1, slotCoordinates);
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


    private async Task TakeSecondTurn()
    {
        MoveLeaderRight();
        Card succesfulSummon;
        succesfulSummon = await PlayCardByCost(2, "12");
        if (succesfulSummon == null)
        {
            await PlayCardByCost(1, "12");
        }
        succesfulSummon = await PlayCardByCost(3, "01");
        if (succesfulSummon == null)
        {
            succesfulSummon = await PlayCardByCost(2, "01");
            if (succesfulSummon == null)
            {
                await PlayCardByCost(1, "01");
            }
        }
    }

    private void MoveLeaderRight()
    {
        CardObject leaderCardObject = Game.INSTANCE.FindEnemyLeaderCardObject(Game.INSTANCE.EnemyLeader);
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
            rightSlot = Game.INSTANCE.GetSlotMap()[locationY.ToString() + (locationX + 1).ToString()];
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
        Game.INSTANCE.RecalculateCosts();
        Game.INSTANCE.UpdateStatBoxes(card, rightSlot, previousSlot);
    }

    private async Task TakeFirstTurn()
    {
        MoveLeaderTop();
        Card succesfulSummon;
        await PlayCardByCost(1, "11");
        await PlayCardByCost(1, "00");
        succesfulSummon = await PlayCardByCost(2, "10");
        if (succesfulSummon == null)
        {
            await PlayCardByCost(1, "10");
        }
    }

    private async Task AllAttack()
    {
        for(int index = 0; index<Game.INSTANCE.CardObjectsInGame.Count; index++) 
        {
            CardObject co = Game.INSTANCE.CardObjectsInGame[index];
            if (co.IsEnemyCard())
            {
                CardObject bestTarget = GetBestTarget(co);
                if(bestTarget == null)
                {
                    continue;
                }
                GameObject targetCardSlot = bestTarget.GameObject.transform.parent.gameObject;
                bool destroysCard = Game.INSTANCE.Attack(targetCardSlot, co.card.Attack + co.counters);
                if (destroysCard)
                {
                    EffectListener.INSTANCE.OnDestroyedCard(co);
                    GameObject enemyCardGO = Game.INSTANCE.GetCardGameObject(targetCardSlot);
                    Image goImage = enemyCardGO.GetComponent<Image>();
                    goImage.color = new Color32(255, 255, 255, 255);
                    Game.INSTANCE.CardObjectsInGame.Remove(bestTarget);
                    enemyCardGO.transform.SetParent(Game.INSTANCE.PlayerGraveyard.transform, false);
                    for(int i=0; i< bestTarget.GameObject.transform.childCount; i++)
                    {
                        GameObject child = bestTarget.GameObject.transform.GetChild(i).gameObject;
                        if (child.name.StartsWith("CounterPanel") || child.name == "Shield")
                        {
                            GameObject.Destroy(child);
                        }
                    }
                    if (bestTarget.card.ImageName == Game.INSTANCE.PlayerLeader)
                    {
                        Game.INSTANCE.Defeat();
                    }
                    Game.INSTANCE.UpdateStatBoxes(bestTarget, null, previousParent: targetCardSlot);
                    Game.INSTANCE.RecalculateCosts();
                    await Task.Delay(1000);
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
            if (Game.INSTANCE.SlotWithCard(slot))
            {
                bool canAttack = Game.INSTANCE.CanAttack(co.GameObject.transform.parent.gameObject, slot);
                if (canAttack)
                {
                    CardObject candidate = Game.INSTANCE.FindCardObject(Game.INSTANCE.GetCardGameObject(slot));
                    if(candidate == null)
                    {
                        continue;
                    }
                    if (bestTarget == null)
                    {
                        bestTarget = candidate;
                    }
                    if(candidate.card.ImageName == Game.INSTANCE.PlayerLeader)
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
                bool canAttack = Game.INSTANCE.CanAttack(cardSlot, Game.INSTANCE.GetSlotMap()[i.ToString()+j.ToString()]);
                if (canAttack)
                {
                    attackOptions.Add(Game.INSTANCE.GetSlotMap()[i.ToString() + j.ToString()]);
                }
            }
        }
        return attackOptions;
    }

    private async Task UseEffects()
    {
        List<CardObject> cin = new List<CardObject>(Game.INSTANCE.CardObjectsInGame);
        foreach (CardObject co in cin) 
        {
            if (co.IsEnemyCard())
            {
                if (co.card.PassiveEffect)
                {
                    continue;
                }
                Card c = co.card;
                c.InitializeEffectParametrs();
                c.SetUsedByPlayer(false);
                if (!co.card.UsesTarget)
                {
                    Game.INSTANCE.UseCardEffect(co, null);
                }
                else
                {
                    if (co.card.RequiresAI)
                    {
                        SetBestEffectTarget(co.card);
                        Game.INSTANCE.UseCardEffect(co, null);
                    }
                    else
                    {
                        TagType targetTag = co.card.TargetTag;
                        CardObject target = GetCardInFieldByTag(targetTag, co.card.TargetType);
                        if (target != null)
                        {
                            Game.INSTANCE.UseCardEffect(co, target);
                        }
                    }
                }
            }
            await Task.Delay(1000);
        }
    }

    private void SetBestEffectTarget(Card effectUser)
    {
        if (effectUser.ImageName == "Nanjo")
        {
            NanjoEffectAI(effectUser);
        }
        else if(effectUser.ImageName == "Shannon")
        {
            ShannonEffectAI(effectUser);
        }
        return;
    }

    private void NanjoEffectAI(Card effectUser)
    {
        CardObject bestTarget = null;
        for (int i = 0; i < Game.INSTANCE.CardObjectsInGame.Count; i++)
        {
            CardObject candidate = Game.INSTANCE.CardObjectsInGame[i];
            if (candidate.GameObject.transform.parent.parent.name != Game.INSTANCE.EnemyField.name)
            {
                continue;
            }
            else if (candidate.card.ImageName == Game.INSTANCE.EnemyLeader)
            {
                bestTarget = candidate;
                break;
            }
            else
            {
                if (bestTarget == null)
                {
                    bestTarget = candidate;
                }
                else
                {
                    if ((bestTarget.card.HP + bestTarget.counters - bestTarget.currentHP) < (candidate.card.HP + candidate.counters - candidate.currentHP))
                    {
                        bestTarget = candidate;
                    }
                }
            }
        }
        effectUser.SetTargetCardObject(bestTarget);
    }

    private void ShannonEffectAI(Card effectUser)
    {
        CardObject bestTarget = null;
        bool cardsInFrontRow = false;
        for (int i = 0; i < Game.INSTANCE.CardObjectsInGame.Count; i++)
        {
            CardObject candidate = Game.INSTANCE.CardObjectsInGame[i];
            if (candidate.GameObject.transform.parent.parent.name != Game.INSTANCE.EnemyField.name || Game.INSTANCE.HasShield(candidate.GameObject))
            {
                continue;
            }
            string yPosition = candidate.GameObject.transform.parent.name.Substring(8, 1);
            if (yPosition == "1")
            {
                if (bestTarget == null || cardsInFrontRow == false)
                {
                    bestTarget = candidate;
                }
                else
                {
                    if (candidate.currentATK > bestTarget.currentATK)
                    {
                        bestTarget = candidate;
                    }
                }
                cardsInFrontRow = true;
            }
            else
            {
                if (cardsInFrontRow)
                {
                    continue;
                }
                else
                {
                    if (bestTarget == null)
                    {
                        bestTarget = candidate;
                    }
                    else
                    {
                        if (candidate.currentATK > bestTarget.currentATK)
                        {
                            bestTarget = candidate;
                        }
                    }
                }
            }
        }
        effectUser.SetTargetCardObject(bestTarget);
    }

    private CardObject GetCardInFieldByTag(TagType targetTag, TargetType targetType)
    {
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if ((co.card.Tags.Contains(targetTag) || targetTag == TagType.All) && (TargetType.Both == targetType || co.GameObject.transform.parent.parent.name == Game.INSTANCE.EnemyField.name))
            {
                return co;
            }
        }
        return null;
    }

    private async Task<Card> PlayCardByCost(int cost, string slotNumber)
    {
        Card card = GetCardByCostFromHand(cost);
        if (card != null)
        {
            Card ret = await PlayCardInSlot(card, slotNumber);
            if (ret!=null)
            {
                List<CardObject> co = Game.INSTANCE.FindCardObject(ret.ImageName);
                if(co.Count != 0)
                {
                    EffectListener.INSTANCE.OnCardPlayed(co[0]);
                }
                Game.INSTANCE.RecalculateCosts();
            }
            return ret;
        }
        return null;
    }

    public async Task<Card> PlayCardInSlot(Card card, string slotNumber)
    {
        GameObject slot = GameObject.Find("CardSlot" + slotNumber);
        if (SlotOccupied(slot))
        {
            return null;
        }
        GameObject slotBox = slot.transform.GetChild(2).GetChild(0).gameObject;
        TextMeshProUGUI slotText = slotBox.GetComponent<TextMeshProUGUI>();
        if(card.Cost > Int32.Parse(slotText.text))
        {
            return null;
        }
        CardObject co = Game.INSTANCE.PlayCardInSlot(card.ImageName, slot);
        Game.INSTANCE.CardObjectsInGame.Add(co);
        await Task.Delay(1000);
        return co.card;
    }

    private Card GetCardByCostFromHand(int cost)
    {
        for (int i = 0; i< Game.INSTANCE.EnemyHandArea.transform.childCount; i++)
        {
            GameObject cardInHand = Game.INSTANCE.EnemyHandArea.transform.GetChild(i).gameObject;
            string cardName = cardInHand.GetComponent<Image>().sprite.name;
            Card c = Game.INSTANCE.FindCard(cardName);
            if (c.Cost == cost)
            {
                return c;
            }
        }
        return null;
    }

    private void MoveLeaderTop()
    {
        CardObject leaderCardObject = Game.INSTANCE.FindEnemyLeaderCardObject(Game.INSTANCE.EnemyLeader);
        GameObject leaderGameObject = leaderCardObject.GameObject;
        GameObject slot = leaderGameObject.transform.parent.gameObject;
        int locationY = Int32.Parse(slot.name.Substring(8, 1));
        int locationX = Int32.Parse(slot.name.Substring(9, 1));
        MoveCardUp(leaderCardObject, locationX, locationY, slot);

    }

    private void MoveCardUp(CardObject card, int currentX, int currentY, GameObject previousSlot)
    {
        GameObject upSlot = Game.INSTANCE.GetSlotMap()[(currentY-1).ToString() + currentX.ToString()];
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
        Game.INSTANCE.RecalculateCosts();
        Game.INSTANCE.UpdateStatBoxes(card, upSlot, previousSlot);
    }


    private bool SlotOccupied(GameObject go)
    {
        return go.transform.childCount > 3;
    }

    private bool IsLeaderOnTop()
    {
        List<CardObject> allCards= Game.INSTANCE.CardObjectsInGame;
        CardObject enemyLeader = allCards.Find(x => x.GameObject.name == Game.INSTANCE.EnemyLeader + "Card");
        string slotName = enemyLeader.GameObject.transform.parent.name;
        int yCoordinates = int.Parse(slotName.Substring(8, 1));
        return yCoordinates == 0;
    }

    private bool IsLeaderOnRight()
    {
        List<CardObject> allCards = Game.INSTANCE.CardObjectsInGame;
        CardObject enemyLeader = allCards.Find(x => x.GameObject.name == Game.INSTANCE.EnemyLeader + "Card");
        string slotName = enemyLeader.GameObject.transform.parent.name;
        int xCoordinates = int.Parse(slotName.Substring(9, 1));
        return xCoordinates == 2;
    }

}
