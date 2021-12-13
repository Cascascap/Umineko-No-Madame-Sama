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
        List<Card> drawnCards =  Game.INSTANCE.Draw(Game.INSTANCE.PlayerDeck, 1, Deck.TagType.Stake);
        if(drawnCards.Count == 0)
        {
            return false;
        }
        Game.INSTANCE.RearrangeHand(true);
        return true;
    }

    internal static bool AsmodeusEffect(Card c)
    {
        CardObject targetCardObject = c.GetTargetCardObject();
        Game.INSTANCE.AddCounterEffect(targetCardObject, 1);
        return true;
    }

    internal static bool BeelzebubEffect(Card c)
    {
        CardObject targetCardObject = c.GetTargetCardObject();
        int counters = c.GetCounters();
        Game.INSTANCE.AddCounter(targetCardObject, counters);
        return true;
    }

    internal static bool LionEffect(Card c)
    {
        GameObject WillGO = null;
        GameObject emptySlot = Game.INSTANCE.GetEmptySlot(Game.INSTANCE.EnemyField);
        bool willInGame = false;
        if (emptySlot == null)
        {
            return false;
        }
        foreach(Card cardInGame in Game.INSTANCE.CardsInGame)
        {
            if(cardInGame.ImageName == "Will")
            {
                List<CardObject> cos = Game.INSTANCE.FindCardObject(cardInGame.ImageName);
                if(cos.Count != 0)
                {
                    willInGame = true;
                    return false;
                }
            }
        }
        if (!willInGame)
        {
            for(int i=0; i<Game.INSTANCE.EnemyGraveyard.transform.childCount; i++)
            {
                GameObject child = Game.INSTANCE.EnemyGraveyard.transform.GetChild(i).gameObject;
                if (child.name.Contains("Will"))
                {
                    WillGO = child;
                    break;
                }
            }
            if(WillGO == null)
            {
                for (int i = 0; i < Game.INSTANCE.EnemyHandArea.transform.childCount; i++)
                {
                    GameObject child = Game.INSTANCE.EnemyHandArea.transform.GetChild(i).gameObject;
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
                List<Card> willCardList = Game.INSTANCE.Draw(Game.INSTANCE.EnemyDeck, 1, cardName:"Will");
                Card willCard = willCardList[0];
                Game.INSTANCE.CreateCardInSlot(willCard.ImageName, emptySlot);
                WillGO = Game.INSTANCE.GetCardGameObject(emptySlot); 
                CardObject cow = new CardObject(WillGO, willCard);
                Game.INSTANCE.CardObjectsInGame.Add(cow);
                Game.INSTANCE.UpdateStatBoxes(cow, emptySlot);
                Game.INSTANCE.RecalculateCosts();
                return true;
            }
            WillGO.transform.SetParent(emptySlot.transform, false);
            WillGO.transform.localPosition = new Vector3(0, 0, 0);
            Card willCard2 = Game.INSTANCE.FindCard("Will");
            CardObject co = new CardObject(WillGO, willCard2);
            Game.INSTANCE.CardObjectsInGame.Add(co);
            Game.INSTANCE.UpdateStatBoxes(co, emptySlot);
            Game.INSTANCE.RecalculateCosts();
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
        Game.INSTANCE.DamageAllEnemyCards(4, c.GetUsedByPlayer());
        Game.INSTANCE.UpdateAllStatBoxes();
        return true;
    }

    internal static bool LeviathanEffect(Card c)
    {
        Debug.Log("");
        CardObject co = c.GetTargetCardObject();
        if (co.counters == 0)
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
            Game.INSTANCE.RemoveCounter(co, countersToRemove);
            CardObject leviathanCO = Game.INSTANCE.FindCardObject(Game.INSTANCE.SelectedCardGameObject);
            Game.INSTANCE.AddCounter(leviathanCO, countersToRemove);
            return true;
        }
    }

    internal static bool LuciferEffect(Card c)
    {
        List<CardObject> stakes = Game.INSTANCE.FindCardsInGameByTag(Deck.TagType.Stake);
        CardObject lucifer = c.GetTargetCardObject();
        Game.INSTANCE.AddCounter(lucifer, (stakes.Count-1)*2);
        return true;
    }

    internal static bool MammonEffect(Card c)
    {
        Game.INSTANCE.Draw(Game.INSTANCE.PlayerDeck, 1);
        Game.INSTANCE.RearrangeHand(true);
        return true;
    }

    internal static bool GenjiEffect(Card arg)
    {
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.card.ImageName == "Kinzo")
            {
                Game.INSTANCE.AddCounterEffect(co, 3);
                return true;
            }
        }
        return false;
    }

    internal static bool KinzoEffect(Card c)
    {
        CardObject targetCardObject = c.GetTargetCardObject();
        Game.INSTANCE.AddCounterEffect(targetCardObject, 2);
        return true;
    }

    internal static bool ShannonEffect(Card c)
    {
        Game.INSTANCE.CreateShield(c.GetTargetCardObject().GameObject);
        return true;
    }

    internal static bool GohdaEffect(Card arg)
    {
        foreach(CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if(co.GameObject.transform.parent.parent.name == "EnemyField")
            {
                Game.INSTANCE.AddCounterEffect(co, 1);
            }
        }
        return true;
    }

    internal static bool KanonEffect(Card c)
    {
        CardObject kanon = c.GetTargetCardObject();
        Game.INSTANCE.AddCounter(kanon, 1);
        return true;
    }

    internal static bool SatanEffect(Card c)
    {
        CardObject satanCO = c.GetTargetCardObject();
        Game.INSTANCE.AddCounter(satanCO, 1);
        return true;
    }

    internal static bool KumasawaEffect(Card c)
    {
        Game.INSTANCE.DamageAllEnemyCards(1, c.GetUsedByPlayer());
        Game.INSTANCE.UpdateAllStatBoxes();
        return true;
    }

    internal static bool NanjoEffect(Card c)
    {
        CardObject bestTarget = c.GetTargetCardObject();
        bestTarget.currentHP = bestTarget.card.HP + bestTarget.counters;
        Game.INSTANCE.UpdateStatBoxes(bestTarget, bestTarget.GameObject.transform.parent.gameObject);
        return true;
    }

    internal static bool RonoveEffect(Card c)
    {
        Game.INSTANCE.CreateShield(c.GetTargetCardObject().GameObject);
        return true;
    }

    internal static bool WillEffect(Card c)
    {
        Debug.Log("Witches cant use their skill");
        List<TagType> tags = c.GetTargetCardTags();
        CardObject effectUser = c.GetTargetCardObject();
        if (tags.Contains(TagType.Witch) && effectUser.GameObject.transform.parent.parent.name == "PlayerField")
        {
            return false;
        }
        return true;
    }

    internal static bool DianaEffect(Card c)
    {
        List<CardObject> willcos = Game.INSTANCE.FindCardObject("Will");
        if(willcos.Count == 0)
        {
            return false;
        }
        CardObject willco = willcos[0];
        Game.INSTANCE.AddCounter(willco, 2);
        return true;
    }

    internal static bool KonpeitouEffect(Card c)
    {
        Debug.Log("");
        return true;
    }

    internal static bool LambdaEffect(Card c)
    {
        GameObject gameSlot = Game.INSTANCE.FindFreeSlot(true);
        if (gameSlot != null)
        {
            CardObject co = Game.INSTANCE.CreateCardInSlot("Konpeitou", gameSlot);
            Game.INSTANCE.CardObjectsInGame.Add(co);
            return true;
        }
        else
        {
            return false;
        }
    }

    internal static bool BelphegorEffect(Card c)
    {
        CardObject targetCardObject = c.GetTargetCardObject();
        if (!targetCardObject.acted)
        {
            targetCardObject.currentHP = targetCardObject.card.HP + targetCardObject.counters;
            Game.INSTANCE.UpdateStatBoxes(targetCardObject, targetCardObject.GameObject.transform.parent.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }
}
