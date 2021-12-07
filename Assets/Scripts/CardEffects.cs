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
        GameObject WillGO = null;
        GameObject emptyField = GameStart.INSTANCE.GetEmptySlot(GameStart.INSTANCE.EnemyField);
        bool willInGame = false;
        if (emptyField == null)
        {
            return false;
        }
        foreach(Card cardInGame in GameStart.INSTANCE.CardsInGame)
        {
            if(cardInGame.ImageName == "Will")
            {
                List<CardObject> cos = GameStart.INSTANCE.FindCardObject(cardInGame);
                if(cos.Count != 0)
                {
                    willInGame = true;
                    return false;
                }
            }
        }
        if (!willInGame)
        {
            for(int i=0; i<GameStart.INSTANCE.EnemyGraveyard.transform.childCount; i++)
            {
                GameObject child = GameStart.INSTANCE.EnemyGraveyard.transform.GetChild(i).gameObject;
                if (child.name.Contains("Will"))
                {
                    WillGO = child;
                    break;
                }
            }
            if(WillGO == null)
            {
                for (int i = 0; i < GameStart.INSTANCE.EnemyHandArea.transform.childCount; i++)
                {
                    GameObject child = GameStart.INSTANCE.EnemyHandArea.transform.GetChild(i).gameObject;
                    if (child.name.Contains("Will"))
                    {
                        WillGO = child;
                        break;
                    }
                }
            }
            if(WillGO == null)
            {
                List<Card> willCardList = GameStart.INSTANCE.Draw(GameStart.INSTANCE.EnemyDeck, 1, cardName:"Will");
                Card willCard = willCardList[0];
                GameStart.INSTANCE.CreateCardInSlot(willCard.ImageName, emptyField);
                WillGO = emptyField.transform.GetChild(3).gameObject; 
                CardObject cow = new CardObject(WillGO, willCard);
                GameStart.INSTANCE.CardObjectsInGame.Add(cow);
                GameStart.INSTANCE.UpdateStatBoxes(cow, emptyField);
                GameStart.INSTANCE.RecalculateCosts();
                return true;
            }
            WillGO.transform.SetParent(emptyField.transform, false);
            WillGO.transform.localPosition = new Vector3(0, 0, 0);
            GameObject cardCover = WillGO.transform.GetChild(0).gameObject;
            GameObject.DestroyImmediate(cardCover);
            Card willCard2 = GameStart.INSTANCE.FindCard("Will");
            CardObject co = new CardObject(WillGO, willCard2);
            GameStart.INSTANCE.CardObjectsInGame.Add(co);
            GameStart.INSTANCE.UpdateStatBoxes(co, emptyField);
            GameStart.INSTANCE.RecalculateCosts();
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
        CardObject targetCardObject = GameStart.INSTANCE.FindCardObject(c.GetTargetCard());
        GameStart.INSTANCE.CreateShield(targetCardObject.GameObject);
        return true;
    }

    internal static bool WillEffect(Card c)
    {
        Debug.Log("Witches cant use their skill");
        List<TagType> tags = c.GetTargetCardTags();
        GameObject effectUser = c.GetTargetCard();
        if (tags.Contains(TagType.Witch) && effectUser.transform.parent.parent.name == "PlayerField")
        {
            return false;
        }
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
