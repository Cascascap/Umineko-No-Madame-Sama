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
        GameStart.INSTANCE.AddCounter(targetCardObject,  1);
    }

    internal static void BeelzebubEffect(GameObject go)
    {
        throw new NotImplementedException();
    }

    internal static void LionEffect(GameObject go)
    {
        throw new NotImplementedException();
    }

    internal static void GaapEffect(GameObject go)
    {
        throw new NotImplementedException();
    }

    internal static void GoatEffect(GameObject go)
    {
        return;
    }

    internal static void VirgiliaEffect(GameObject go)
    {
        throw new NotImplementedException();
    }

    internal static void LeviathanEffect(GameObject go)
    {
        throw new NotImplementedException();
    }

    internal static void LuciferEffect(GameObject go)
    {
        throw new NotImplementedException();
    }

    internal static void MammonEffect(GameObject go)
    {
        GameStart.INSTANCE.Draw(GameStart.INSTANCE.PlayerDeck, 1);
        GameStart.INSTANCE.RearrangeHand(true);
    }

    internal static void SatanEffect(GameObject go)
    {
        throw new NotImplementedException();
    }

    internal static void RonoveEffect(GameObject go)
    {
        throw new NotImplementedException();
    }

    internal static void WillEffect(GameObject go)
    {
        throw new NotImplementedException();
    }

    internal static void DianaEffect(GameObject go)
    {
        throw new NotImplementedException();
    }

    internal static void KonpeitouEffect(GameObject go)
    {
        throw new NotImplementedException();
    }

    internal static void LambdaEffect(GameObject go)
    {
        Debug.Log("Lambda's Effect");
    }

    internal static void BelphegorEffect(GameObject go)
    {
        throw new NotImplementedException();
    }
}
