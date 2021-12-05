using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffects 
{
    public static void BeatriceEffect(GameObject go)
    {
        Debug.Log("Draw stake");
    }

    internal static void AsmodeusEffect(GameObject go)
    {
        Debug.Log("Once per turn: Gives a summoned card a +1/+1 counter");
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(go);
        GameStart.INSTANCE.AddCounterEffect(targetCardObject,  1);
    }

    internal static void BeelzebubEffect(GameObject go)
    {
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(go);
        GameStart.INSTANCE.AddCounter(targetCardObject, 1);
    }

    internal static void LionEffect(GameObject go)
    {
        Debug.Log("");
    }

    internal static void GaapEffect(GameObject go)
    {
        Debug.Log("");
    }

    internal static void GoatEffect(GameObject go)
    {
        Debug.Log("");
    }

    internal static void VirgiliaEffect(GameObject go)
    {
        Debug.Log("");
    }

    internal static void LeviathanEffect(GameObject go)
    {
        Debug.Log("");
    }

    internal static void LuciferEffect(GameObject go)
    {
        Debug.Log("");
    }

    internal static void MammonEffect(GameObject go)
    {
        GameStart.INSTANCE.Draw(GameStart.INSTANCE.PlayerDeck, 1);
        GameStart.INSTANCE.RearrangeHand(true);
    }

    internal static void SatanEffect(GameObject go)
    {
        Debug.Log("");
    }

    internal static void RonoveEffect(GameObject go)
    {
        Debug.Log("");
    }

    internal static void WillEffect(GameObject go)
    {
        Debug.Log("");
    }

    internal static void DianaEffect(GameObject go)
    {
        Debug.Log("Grants will 2 +1/+1 counter at the start of your turn");
    }

    internal static void KonpeitouEffect(GameObject go)
    {
        Debug.Log("");
    }

    internal static void LambdaEffect(GameObject go)
    {
        GameObject gameSlot = GameStart.INSTANCE.FindFreeSlot(true);
        if (gameSlot != null)
        {
            CardObject co = GameStart.INSTANCE.CreateCardInSlot("Konpeitou", gameSlot);
            GameStart.INSTANCE.CardGameObjectsInGame.Add(co);
        }
    }

    internal static void BelphegorEffect(GameObject go)
    {
        Debug.Log("");
    }
}
