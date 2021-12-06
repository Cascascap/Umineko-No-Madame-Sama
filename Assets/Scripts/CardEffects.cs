using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log("Once per turn: Gives a summoned card a +1/+1 counter");
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(c.GetTargetCard());
        GameStart.INSTANCE.AddCounterEffect(targetCardObject, 1);
        return true;
    }

    internal static bool BeelzebubEffect(Card c)
    {
        GameObject target = c.GetTargetCard();
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(target);
        int countersToRemove = targetCardObject.counters;
        if(countersToRemove >= 2) 
        {
            countersToRemove = 2;
        }
        GameStart.INSTANCE.AddCounter(targetCardObject, countersToRemove);
        return true;
    }

    internal static bool LionEffect(Card c)
    {
        Debug.Log("");
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
        GameStart.INSTANCE.DamageAllEnemyCards(4);
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
            GameStart.INSTANCE.RemoveCounter(co, 1);
            CardObject leviathanCO = GameStart.INSTANCE.FindCardObject(GameStart.INSTANCE.SelectedCardGameObject);
            GameStart.INSTANCE.AddCounter(leviathanCO, 1);
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

    internal static bool SatanEffect(Card c)
    {
        GameObject target = c.GetTargetCard();
        CardObject satanCO = GameStart.INSTANCE.FindCardObject(target);
        GameStart.INSTANCE.AddCounter(satanCO, 1);
        return true;
    }

    internal static bool RonoveEffect(Card c)
    {
        Debug.Log("Gives ally card a shield");
        return true;
    }

    internal static bool WillEffect(Card c)
    {
        Debug.Log("Witches cant use their skill");
        return true;
    }

    internal static bool DianaEffect(Card c)
    {
        Debug.Log("Grants will 2 +1/+1 counter at the start of your turn");
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
