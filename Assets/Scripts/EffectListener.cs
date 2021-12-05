using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectListener 
{

    public static EffectListener INSTANCE = null;
    public List<Card> GettingCountersList = new List<Card>(); 

    public EffectListener()
    {
        INSTANCE = this;
    }

    public bool OnGettingCounters(GameObject go, int countersAdded)
    {
        foreach(Card c in GettingCountersList)
        {
            if (go.name.StartsWith(c.ImageName))
            {
                c.Effect.Invoke(go);
            }
        }
        return true;
    }
}
