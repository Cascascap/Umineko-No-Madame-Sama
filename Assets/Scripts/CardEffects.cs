using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Deck;

public class CardEffects
{
    internal static bool BeatriceEffect(Card c)
    {
        Debug.Log("Draw stake");
        List<Card> drawnCards =  GameStart.INSTANCE.Draw(GameStart.INSTANCE.PlayerDeck, 1, Deck.TagType.Stake);
        if(drawnCards.Count == 0)
        {
            return false;
        }
        GameStart.INSTANCE.RearrangeHand(true);
        return true;
    }

    internal static bool AsmodeusEffect(Card c)
    {
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(c.GetTargetCard());
        GameStart.INSTANCE.AddCounterEffect(targetCardObject, 1);
        return true;
    }

    internal static bool BeelzebubEffect(Card c)
    {
        GameObject target = c.GetTargetCard();
        int counters = c.GetCounters();
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(target);
        GameStart.INSTANCE.AddCounter(targetCardObject, counters);
        return true;
    }

    internal static bool LionEffect(Card c)
    {
        GameObject WillGO = null;
        GameObject emptySlot = GameStart.INSTANCE.GetEmptySlot(GameStart.INSTANCE.EnemyField);
        bool willInGame = false;
        if (emptySlot == null)
        {
            return false;
        }
        foreach(Card cardInGame in GameStart.INSTANCE.CardsInGame)
        {
            if(cardInGame.ImageName == "Will")
            {
                List<CardObject> cos = GameStart.INSTANCE.FindCardObject(cardInGame.ImageName);
                if(cos.Count != 0)
                {
                    willInGame = true;
                    return false;
                }
            }
        }
        if (!willInGame)
        {
            for(int i=0; i<GameStart.INSTANCE.EnemyGraveyard.transform.childCount; i++)
            {
                GameObject child = GameStart.INSTANCE.EnemyGraveyard.transform.GetChild(i).gameObject;
                if (child.name.Contains("Will"))
                {
                    WillGO = child;
                    break;
                }
            }
            if(WillGO == null)
            {
                for (int i = 0; i < GameStart.INSTANCE.EnemyHandArea.transform.childCount; i++)
                {
                    GameObject child = GameStart.INSTANCE.EnemyHandArea.transform.GetChild(i).gameObject;
                    if (child.name.Contains("Will"))
                    {
                        WillGO = child;
                        GameObject cardCover = WillGO.transform.GetChild(0).gameObject;
                        GameObject.DestroyImmediate(cardCover);
                        break;
                    }
                }
            }
            if(WillGO == null)
            {
                List<Card> willCardList = GameStart.INSTANCE.Draw(GameStart.INSTANCE.EnemyDeck, 1, cardName:"Will");
                Card willCard = willCardList[0];
                GameStart.INSTANCE.CreateCardInSlot(willCard.ImageName, emptySlot);
                WillGO = GameStart.INSTANCE.GetCardGameObject(emptySlot); 
                CardObject cow = new CardObject(WillGO, willCard);
                GameStart.INSTANCE.CardObjectsInGame.Add(cow);
                GameStart.INSTANCE.UpdateStatBoxes(cow, emptySlot);
                GameStart.INSTANCE.RecalculateCosts();
                return true;
            }
            WillGO.transform.SetParent(emptySlot.transform, false);
            WillGO.transform.localPosition = new Vector3(0, 0, 0);
            Card willCard2 = GameStart.INSTANCE.FindCard("Will");
            CardObject co = new CardObject(WillGO, willCard2);
            GameStart.INSTANCE.CardObjectsInGame.Add(co);
            GameStart.INSTANCE.UpdateStatBoxes(co, emptySlot);
            GameStart.INSTANCE.RecalculateCosts();
        }
        return true;
    }

    internal static bool GaapEffect(Card c)
    {
        return true;
    }

    internal static bool GoatEffect(Card c)
    {
        return true;
    }

    internal static bool VirgiliaEffect(Card c)
    {
        GameStart.INSTANCE.DamageAllEnemyCards(4, true);
        GameStart.INSTANCE.UpdateAllStatBoxes();
        return true;
    }

    internal static bool LeviathanEffect(Card c)
    {
        Debug.Log("");
        GameObject target = c.GetTargetCard();
        CardObject co = GameStart.INSTANCE.FindCardObject(target);
        if(co.counters == 0)
        {
            return false;
        }
        else
        {
            int countersToRemove = co.counters;
            if (countersToRemove >= 2)
            {
                countersToRemove = 2;
            }
            GameStart.INSTANCE.RemoveCounter(co, countersToRemove);
            CardObject leviathanCO = GameStart.INSTANCE.FindCardObject(GameStart.INSTANCE.SelectedCardGameObject);
            GameStart.INSTANCE.AddCounter(leviathanCO, countersToRemove);
            return true;
        }
    }

    internal static bool LuciferEffect(Card c)
    {
        List<CardObject> stakes = GameStart.INSTANCE.FindCardsInGameByTag(Deck.TagType.Stake);
        GameObject luciferGO = c.GetTargetCard();
        CardObject lucifer = GameStart.INSTANCE.FindCardObject(luciferGO);
        GameStart.INSTANCE.AddCounter(lucifer, (stakes.Count-1)*2);
        return true;
    }

    internal static bool MammonEffect(Card c)
    {
        GameStart.INSTANCE.Draw(GameStart.INSTANCE.PlayerDeck, 1);
        GameStart.INSTANCE.RearrangeHand(true);
        return true;
    }

    internal static bool GenjiEffect(Card arg)
    {
        foreach (CardObject co in GameStart.INSTANCE.CardObjectsInGame)
        {
            if (co.card.ImageName == "Kinzo")
            {
                GameStart.INSTANCE.AddCounterEffect(co, 3);
                return true;
            }
        }
        return false;
    }

    internal static bool KinzoEffect(Card arg)
    {
        Debug.Log("Once per turn Gives a servant card 2 +1/+1");
        return true;
    }

    internal static bool ShannonEffect(Card arg)
    {
        Debug.Log("Once per turn: Grants a shield to an ally card");
        CardObject bestTarget = null;
        bool cardsInFrontRow = false;
        for(int i=0; i<GameStart.INSTANCE.CardObjectsInGame.Count; i++)
        {
            CardObject candidate = GameStart.INSTANCE.CardObjectsInGame[i];
            if(candidate.GameObject.transform.parent.parent.name != GameStart.INSTANCE.EnemyField.name || GameStart.INSTANCE.HasShield(candidate.GameObject))
            {
                continue;
            }
            string yPosition = candidate.GameObject.transform.parent.name.Substring(8, 1);
            if (yPosition == "1")
            {
                if(bestTarget == null || cardsInFrontRow==false)
                {
                    bestTarget = candidate;
                }
                else
                {
                    if(candidate.currentATK > bestTarget.currentATK)
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
                    if(bestTarget == null)
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
        GameStart.INSTANCE.CreateShield(bestTarget.GameObject);
        return true;
    }

    internal static bool GohdaEffect(Card arg)
    {
        foreach(CardObject co in GameStart.INSTANCE.CardObjectsInGame)
        {
            if(co.GameObject.transform.parent.parent.name == "EnemyField")
            {
                GameStart.INSTANCE.AddCounterEffect(co, 1);
            }
        }
        return true;
    }

    internal static bool KanonEffect(Card c)
    {
        GameObject target = c.GetTargetCard();
        CardObject kanon = GameStart.INSTANCE.FindCardObject(target);
        GameStart.INSTANCE.AddCounter(kanon, 1);
        return true;
    }

    internal static bool SatanEffect(Card c)
    {
        GameObject target = c.GetTargetCard();
        CardObject satanCO = GameStart.INSTANCE.FindCardObject(target);
        GameStart.INSTANCE.AddCounter(satanCO, 1);
        return true;
    }

    internal static bool KumasawaEffect(Card arg)
    {
        GameStart.INSTANCE.DamageAllEnemyCards(1, false);
        GameStart.INSTANCE.UpdateAllStatBoxes();
        return true;
    }

    internal static bool NanjoEffect(Card arg)
    {
        CardObject bestTarget = null;
        for (int i = 0; i < GameStart.INSTANCE.CardObjectsInGame.Count; i++)
        {
            CardObject candidate = GameStart.INSTANCE.CardObjectsInGame[i];
            if (candidate.GameObject.transform.parent.parent.name != GameStart.INSTANCE.EnemyField.name)
            {
                continue;
            }
            else if(candidate.card.ImageName == GameStart.INSTANCE.EnemyLeader)
            {
                bestTarget = candidate;
                break;
            }
            else
            {
                if(bestTarget == null)
                {
                    bestTarget = candidate;
                }
                else
                {
                    if((bestTarget.card.HP + bestTarget.counters - bestTarget.currentHP) < (candidate.card.HP + candidate.counters - candidate.currentHP))
                    {
                        bestTarget = candidate;
                    }
                }
            }
        }
        bestTarget.currentHP = bestTarget.card.HP + bestTarget.counters;
        GameStart.INSTANCE.UpdateStatBoxes(bestTarget, bestTarget.GameObject.transform.parent.gameObject);
        return true;
    }

    internal static bool RonoveEffect(Card c)
    {
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(c.GetTargetCard());
        GameStart.INSTANCE.CreateShield(targetCardObject.GameObject);
        return true;
    }

    internal static bool WillEffect(Card c)
    {
        Debug.Log("Witches cant use their skill");
        List<TagType> tags = c.GetTargetCardTags();
        GameObject effectUser = c.GetTargetCard();
        if (tags.Contains(TagType.Witch) && effectUser.transform.parent.parent.name == "PlayerField")
        {
            return false;
        }
        return true;
    }

    internal static bool DianaEffect(Card c)
    {
        List<CardObject> willcos = GameStart.INSTANCE.FindCardObject("Will");
        if(willcos.Count == 0)
        {
            return false;
        }
        CardObject willco = willcos[0];
        GameStart.INSTANCE.AddCounter(willco, 2);
        return true;
    }

    internal static bool KonpeitouEffect(Card c)
    {
        Debug.Log("");
        return true;
    }

    internal static bool LambdaEffect(Card c)
    {
        GameObject gameSlot = GameStart.INSTANCE.FindFreeSlot(true);
        if (gameSlot != null)
        {
            CardObject co = GameStart.INSTANCE.CreateCardInSlot("Konpeitou", gameSlot);
            GameStart.INSTANCE.CardObjectsInGame.Add(co);
            return true;
        }
        else
        {
            return false;
        }
    }

    internal static bool BelphegorEffect(Card c)
    {
        GameObject target = c.GetTargetCard();
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(target);
        if (!targetCardObject.acted)
        {
            targetCardObject.currentHP = targetCardObject.card.HP + targetCardObject.counters;
            GameStart.INSTANCE.UpdateStatBoxes(targetCardObject, target.transform.parent.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }
}
