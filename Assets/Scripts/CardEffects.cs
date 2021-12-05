using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffects
{
    public static void BeatriceEffect(Card c)
    {
        Debug.Log("Draw stake");
        GameStart.INSTANCE.Draw(GameStart.INSTANCE.PlayerDeck, 1, Deck.TagType.Stake);
        GameStart.INSTANCE.RearrangeHand(true);
    }

    internal static void AsmodeusEffect(Card c)
    {
        Debug.Log("Once per turn: Gives a summoned card a +1/+1 counter");
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(c.GetTargetCard());
        GameStart.INSTANCE.AddCounterEffect(targetCardObject, 1);
    }

    internal static void BeelzebubEffect(Card c)
    {
        GameObject target = c.GetTargetCard();
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(target);
        GameStart.INSTANCE.AddCounter(targetCardObject, 1);
    }

    internal static void LionEffect(Card c)
    {
        Debug.Log("");
    }

    internal static void GaapEffect(Card c)
    {
        Debug.Log("");
    }

    internal static void GoatEffect(Card c)
    {
        Debug.Log("");
    }

    internal static void VirgiliaEffect(Card c)
    {
        Debug.Log("");
    }

    internal static void LeviathanEffect(Card c)
    {
        Debug.Log("");
    }

    internal static void LuciferEffect(Card c)
    {
        Debug.Log("");
    }

    internal static void MammonEffect(Card c)
    {
        GameStart.INSTANCE.Draw(GameStart.INSTANCE.PlayerDeck, 1);
        GameStart.INSTANCE.RearrangeHand(true);
    }

    internal static void SatanEffect(Card c)
    {
        Debug.Log("");
    }

    internal static void RonoveEffect(Card c)
    {
        Debug.Log("");
    }

    internal static void WillEffect(Card c)
    {
        Debug.Log("");
    }

    internal static void DianaEffect(Card c)
    {
        Debug.Log("Grants will 2 +1/+1 counter at the start of your turn");
    }

    internal static void KonpeitouEffect(Card c)
    {
        Debug.Log("");
    }

    internal static void LambdaEffect(Card c)
    {
        GameObject gameSlot = GameStart.INSTANCE.FindFreeSlot(true);
        if (gameSlot != null)
        {
            CardObject co = GameStart.INSTANCE.CreateCardInSlot("Konpeitou", gameSlot);
            GameStart.INSTANCE.CardGameObjectsInGame.Add(co);
        }
    }

    internal static void BelphegorEffect(Card c)
    {
        GameObject target = c.GetTargetCard();
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(target);
        targetCardObject.currentHP = targetCardObject.card.HP + targetCardObject.counters;
    }
}
