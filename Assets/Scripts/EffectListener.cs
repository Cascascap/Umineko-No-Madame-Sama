using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectListener 
{

    public static EffectListener INSTANCE = null;
    public List<Card> GettingCountersList = new List<Card>();
    public List<Card> TurnEndingList = new List<Card>();

    public EffectListener()
    {
        INSTANCE = this;
    }

    public bool OnTrunEnd()
    {
        foreach (Card c in TurnEndingList)
        {
            //c.Effect.Invoke(null, c);
        }
        return true;
    }

    public bool OnGettingCounters(GameObject go, int countersAdded)
    {
        foreach(Card c in GettingCountersList)
        {
            if (go.name.StartsWith(c.ImageName))
            {
                c.InitializeEffectParametrs();
                c.SetTargetCard(go);
                c.Effect.Invoke(c);
            }
        }
        return true;
    }
}
