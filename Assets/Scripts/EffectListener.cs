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
            List<CardObject> cos = GameStart.INSTANCE.FindCardObject(c);
            foreach(CardObject co in cos)
            {
                c.InitializeEffectParametrs();
                c.SetTargetCard(cardUsingEffect.GameObject);
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
            c.SetTargetCard(cardPlayed.GameObject);
            c.Effect.Invoke(c);
        }
        return true;
    }

    public bool OnTrunEnd()
    {
        foreach (Card c in TurnEndingList)
        {
            List<CardObject> co = GameStart.INSTANCE.FindCardObject(c);
            foreach(CardObject cin in co)
            {
                c.InitializeEffectParametrs();
                c.SetTargetCard(cin.GameObject);
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
            c.SetTargetCard(destroyer.GameObject);
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
                c.SetTargetCard(go);
                c.SetCounters(countersAdded);
                c.Effect.Invoke(c);
            }
        }
        return true;
    }
}
