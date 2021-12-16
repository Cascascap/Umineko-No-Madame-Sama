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
    public List<Card> MovementStopperList = new List<Card>();

    public EffectListener()
    {
        INSTANCE = this;
    }

    public bool OnAllowMovement(CardObject cardMoving)
    {
        bool effectAllowed = true;
        foreach (Card c in MovementStopperList)
        {
            List<CardObject> cos = Game.INSTANCE.FindCardObject(c.ImageName);
            foreach (CardObject co in cos)
            {
                c.InitializeEffectParametrs();
                c.SetTargetCardObject(cardMoving);
                c.SetUsedByPlayer(co.GameObject.transform.parent.parent.name == Game.INSTANCE.PlayerField.name);
                bool effectResult = c.Effect.Invoke(c);
                if (!effectResult)
                {
                    return false;
                }
            }
        }
        return effectAllowed;
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
                c.SetUsedByPlayer(co.GameObject.transform.parent.parent.name == Game.INSTANCE.PlayerField.name);
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
        bool usedByPlayer = cardPlayed.GameObject.transform.parent.parent.name == Game.INSTANCE.PlayerField.name;
        foreach (Card ca in CardPlayedList)
        {
            if(ca == cardPlayed.card && ((usedByPlayer && Game.INSTANCE.GameState != Game.State.EnemyTurn) || (!usedByPlayer && Game.INSTANCE.GameState == Game.State.EnemyTurn)))
            {
                Card c = cardPlayed.card;
                c.InitializeEffectParametrs();
                c.SetUsedByPlayer(usedByPlayer);
                c.SetTargetCardObject(cardPlayed);
                c.Effect.Invoke(c);
            }
            
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
                bool usedByPlayer = cin.GameObject.transform.parent.parent.name == Game.INSTANCE.PlayerField.name;
                if((usedByPlayer && Game.INSTANCE.GameState != Game.State.EnemyTurn) || (!usedByPlayer && Game.INSTANCE.GameState == Game.State.EnemyTurn))
                {
                    c.InitializeEffectParametrs();
                    c.SetUsedByPlayer(usedByPlayer);
                    c.SetTargetCardObject(cin);
                    c.Effect.Invoke(c);
                }
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
            c.SetUsedByPlayer(destroyer.GameObject.transform.parent.parent.name == Game.INSTANCE.PlayerField.name);
            c.SetTargetCardObject(destroyer);
            c.Effect.Invoke(c);
        }
        return true;
    }

    public bool OnGettingCounters(GameObject go, int countersAdded)
    {
        foreach(Card c in GettingCountersList)
        {
            bool usedByPlayer = go.transform.parent.parent.name == Game.INSTANCE.PlayerField.name;
            if ((go.name.StartsWith(c.ImageName) && Game.INSTANCE.FindCardObject(go)!=null) && ((usedByPlayer && Game.INSTANCE.GameState != Game.State.EnemyTurn) || (!usedByPlayer && Game.INSTANCE.GameState == Game.State.EnemyTurn)))
            {
                c.InitializeEffectParametrs();
                CardObject co = Game.INSTANCE.FindCardObject(go);
                c.SetUsedByPlayer(co.GameObject.transform.parent.parent.name == Game.INSTANCE.PlayerField.name);
                c.SetTargetCardObject(co);
                c.SetCounters(countersAdded);
                c.Effect.Invoke(c);
            }
        }
        return true;
    }
}
