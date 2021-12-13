using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectListener 
{

    public static EffectListener INSTANCE = null;
    public List<Card> GettingCountersList = new List<Card>();
    public List<Card> TurnEndingList = new List<Card>();
    public List<Card> CanAttackFromAnywhereList = new List<Card>();
    public List<Card> DestroysCardsList = new List<Card>();
    public List<Card> CardPlayedList = new List<Card>();
    public List<Card> EffectStopperList = new List<Card>();

    public EffectListener()
    {
        INSTANCE = this;
    }

    public bool OnAllowUseEffect(CardObject cardUsingEffect)
    {
        bool effectAllowed = true;
        foreach (Card c in EffectStopperList)
        {
            List<CardObject> cos = Game.INSTANCE.FindCardObject(c.ImageName);
            foreach(CardObject co in cos)
            {
                c.InitializeEffectParametrs();
                c.SetTargetCardObject(cardUsingEffect);
                c.SetTargetCardTags(cardUsingEffect.card.Tags);
                bool effectResult = c.Effect.Invoke(c);
                if (!effectResult)
                {
                    return false;
                }
            }
        }
        return effectAllowed;
    }

    public bool OnCardPlayed(CardObject cardPlayed)
    {
        if (CardPlayedList.Contains(cardPlayed.card))
        {
            Card c = cardPlayed.card;
            c.InitializeEffectParametrs();
            c.SetTargetCardObject(cardPlayed);
            c.Effect.Invoke(c);
        }
        return true;
    }

    public bool OnTrunEnd()
    {
        foreach (Card c in TurnEndingList)
        {
            List<CardObject> co = Game.INSTANCE.FindCardObject(c.ImageName);
            foreach(CardObject cin in co)
            {
                c.InitializeEffectParametrs();
                c.SetTargetCardObject(cin);
                c.Effect.Invoke(c);
            }
        }
        return true;
    }

    public bool OnDestroyedCard(CardObject destroyer)
    {
        if (DestroysCardsList.Contains(destroyer.card))
        {
            Card c = destroyer.card;
            c.InitializeEffectParametrs();
            c.SetTargetCardObject(destroyer);
            c.Effect.Invoke(c);
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
                CardObject co = Game.INSTANCE.FindCardObject(go);
                c.SetTargetCardObject(co);
                c.SetCounters(countersAdded);
                c.Effect.Invoke(c);
            }
        }
        return true;
    }
}
