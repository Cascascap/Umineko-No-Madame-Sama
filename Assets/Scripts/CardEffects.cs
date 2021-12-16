using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static Deck;
using Random = System.Random;

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
        bool usedByPlayer = c.GetUsedByPlayer();
        GameObject graveyard = null;
        GameObject field = null;
        GameObject handArea = null;
        Deck deck = null;
        if (usedByPlayer)
        {
            graveyard = Game.INSTANCE.PlayerGraveyard;
            handArea = Game.INSTANCE.PlayerHandArea;
            field = Game.INSTANCE.PlayerField;
            deck = Game.INSTANCE.PlayerDeck;
        }
        else
        {
            graveyard = Game.INSTANCE.EnemyGraveyard;
            handArea = Game.INSTANCE.EnemyHandArea;
            field = Game.INSTANCE.EnemyField;
            deck = Game.INSTANCE.EnemyDeck;
        }
        GameObject WillGO = null;
        GameObject emptySlot = Game.INSTANCE.GetEmptySlot(field);
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
                    foreach(CardObject co in cos)
                    {
                        if(co.GameObject.transform.parent.parent.name == field.name)
                        {
                            willInGame = true;
                            return false;
                        }
                    }
                }
            }
        }
        if (!willInGame)
        {
            for(int i=0; i<graveyard.transform.childCount; i++)
            {
                GameObject child = graveyard.transform.GetChild(i).gameObject;
                if (child.name.Contains("Will"))
                {
                    WillGO = child;
                    break;
                }
            }
            if(WillGO == null)
            {
                for (int i = 0; i < handArea.transform.childCount; i++)
                {
                    GameObject child = handArea.transform.GetChild(i).gameObject;
                    if (child.name.StartsWith("Will"))
                    {
                        WillGO = child;
                        //If played by the enemy, the card in hand must take off the cover
                        if (!usedByPlayer)
                        {
                            GameObject cardCover = WillGO.transform.GetChild(0).gameObject;
                            GameObject.DestroyImmediate(cardCover);
                        }
                        break;
                    }
                }
            }
            if(WillGO == null)
            {
                List<Card> willCardList = Game.INSTANCE.Draw(deck, 1, cardName:"Will");
                Card willCard = willCardList[0];
                Game.INSTANCE.RemoveCardObjectFromHand(handArea, willCard);
                Game.INSTANCE.CreateCardInSlot(willCard.ImageName, emptySlot);
                WillGO = Game.INSTANCE.GetCardGameObject(emptySlot); 
                CardObject cow = new CardObject(WillGO, willCard);
                Game.INSTANCE.CardObjectsInGame.Add(cow);
                Game.INSTANCE.UpdateStatBoxes(cow, emptySlot);
                Game.INSTANCE.RecalculateCosts();
                Game.INSTANCE.RearrangeHand(usedByPlayer);
                return true;
            }
            WillGO.transform.SetParent(emptySlot.transform, false);
            WillGO.transform.localPosition = new Vector3(0, 0, 0);
            Card willCard2 = Game.INSTANCE.FindCard("Will");
            CardObject co = new CardObject(WillGO, willCard2);
            Game.INSTANCE.CardObjectsInGame.Add(co);
            Game.INSTANCE.UpdateStatBoxes(co, emptySlot);
            Game.INSTANCE.RecalculateCosts();
            Game.INSTANCE.RearrangeHand(usedByPlayer); 
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
            Game.INSTANCE.AddCounterEffect(leviathanCO, countersToRemove);
            return true;
        }
    }

    internal static bool LuciferEffect(Card c)
    {
        List<CardObject> stakes = Game.INSTANCE.FindCardsInGameByTag(Deck.TagType.Stake, c.GetUsedByPlayer());
        if(stakes.Count == 1)
        {
            return false;
        }
        CardObject lucifer = c.GetTargetCardObject();
        Game.INSTANCE.AddCounterEffect(lucifer, (stakes.Count-1)*2);
        return true;
    }

    internal static bool MammonEffect(Card c)
    {
        Deck deck = null;
        if (c.GetUsedByPlayer())
        {
            deck = Game.INSTANCE.PlayerDeck;
        }
        else
        {
            deck = Game.INSTANCE.EnemyDeck;
        }
        Game.INSTANCE.Draw(deck, 1);
        Game.INSTANCE.RearrangeHand(c.GetUsedByPlayer());
        return true;
    }

    internal static bool GenjiEffect(Card c)
    {
        GameObject playerField = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.card.ImageName == "Kinzo" && co.GameObject.transform.parent.parent.name == playerField.name)
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

    internal static bool BattlerEffect(Card c)
    {
        GameObject playerField = GetPlayerField(c);
        CardObject battlerCO = c.GetTargetCardObject();

        List<CardObject> iteratable = new List<CardObject>(Game.INSTANCE.CardObjectsInGame);
        foreach (CardObject co in iteratable)
        {
            if(co.GameObject.transform.parent.parent.name != playerField.name && co.card.Tags.Contains(TagType.Witch))
            {
                Game.INSTANCE.DamageCard(co, battlerCO.counters + battlerCO.card.Attack);
            }
        }
        Game.INSTANCE.UpdateAllStatBoxes();
        return true;
    }

    internal static bool AngeEffect(Card c)
    {
        GameObject graveyard = null;
        if (c.GetUsedByPlayer())
        {
            graveyard = Game.INSTANCE.PlayerGraveyard;
        }
        else
        {
            graveyard = Game.INSTANCE.EnemyGraveyard;
        }
        if(graveyard.transform.childCount < 1)
        {
            return false;
        }
        GameObject freeSlot = Game.INSTANCE.FindFreeSlot(!c.GetUsedByPlayer());
        if (freeSlot != null)
        {
            Game.INSTANCE.ResurrectRandomCard(graveyard, freeSlot);
            if (graveyard.transform.childCount < 1)
            {
                return false;
            }
            GameObject freeSlot2 = Game.INSTANCE.FindFreeSlot(!c.GetUsedByPlayer());
            if(freeSlot2 == null)
            {
                return false;
            }
            Game.INSTANCE.ResurrectRandomCard(graveyard, freeSlot2);
            return true;
        }
        else
        {
            return false;
        }
    }

    internal static bool RudolfEffect(Card c)
    {
        GameObject playerField = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.card.ImageName == "Battler" && co.GameObject.transform.parent.parent.name == playerField.name)
            {
                Game.INSTANCE.AddCounterEffect(co, 3);
                return true;
            }
        }
        return false;
    }

    internal static bool KyrieEffect(Card c)
    {
        GameObject playerField = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.card.ImageName == "Battler" && co.GameObject.transform.parent.parent.name == playerField.name)
            {
                Game.INSTANCE.AddCounterEffect(co, 3);
                return true;
            }
        }
        return false;
    }

    internal static bool EvaEffect(Card c)
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

    internal static bool GeorgeEffect(Card c)
    {

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
            Game.INSTANCE.AddCounterEffect(leviathanCO, countersToRemove);
            return true;
        }
    }

    internal static bool HideyoshiEffect(Card arg)
    {
        return true;
    }

    internal static bool FutureGoatEffect(Card arg)
    {
        return true;
    }

    internal static bool GohdaEffect(Card c)
    {
        GameObject field = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.GameObject.transform.parent.parent.name == field.name)
            {
                Game.INSTANCE.AddCounterEffect(co, 1);
            }
        }
        return true;
    }

    internal static bool CorneliaEffect(Card c)
    {
        GameObject playerField = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.GameObject.transform.parent.parent.name == playerField.name)
            {
                Game.INSTANCE.CreateShield(co.GameObject);
            }
        }
        return true;
    }

    internal static bool GertrudeEffect(Card c)
    {

        GameObject playerField = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.card.ImageName == "Dlanor" && co.GameObject.transform.parent.parent.name == playerField.name)
            {
                Game.INSTANCE.AddCounterEffect(co, 5);
                return true;
            }
        }
        return false;
    }

    internal static bool DlanorEffect(Card c)
    {
        Debug.Log("Leaders cant use their skill");
        List<TagType> tags = c.GetTargetCardTags();
        CardObject effectUser = c.GetTargetCardObject();
        if (tags.Contains(TagType.Leader) && effectUser.GameObject.transform.parent.parent.name == "PlayerField")
        {
            return false;
        }
        return true;
    }

    private static GameObject GetPlayerField(Card c)
    {
        GameObject field = null;
        if (c.GetUsedByPlayer())
        {
            field = Game.INSTANCE.PlayerField;
        }
        else
        {
            field = Game.INSTANCE.EnemyField;
        }

        return field;
    }

    internal static bool Chiester00Effect(Card c)
    {
        List<CardObject> chiesters = Game.INSTANCE.FindCardsInGameByTag(Deck.TagType.Chiester, c.GetUsedByPlayer());
        if (chiesters.Count == 0)
        {
            return false;
        }
        Game.INSTANCE.DamageAllEnemyCards(5 * chiesters.Count, c.GetUsedByPlayer());
        return true;
    }

    internal static bool JessicaEffect(Card c)
    {
        GameObject playerField = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.GameObject.transform.parent.parent.name != playerField.name && Game.INSTANCE.HasShield(co.GameObject))
            {
                Game.INSTANCE.RemoveShield(co.GameObject);
            }
        }
        return true;
    }

    internal static bool Chiester45Effect(Card c)
    {
        Game.INSTANCE.DamageAllEnemyCards(2, c.GetUsedByPlayer(), row:"front");
        return true;
    }

    internal static bool Chiester556Effect(Card c)
    {
        GameObject field = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.GameObject.transform.parent.parent.name == field.name && co.card.Tags.Contains(TagType.Chiester))
            {
                Game.INSTANCE.AddCounterEffect(co, 2);
            }
        }
        return true;
        
    }

    internal static bool Chiester410Effect(Card c)
    {
        Game.INSTANCE.DamageAllEnemyCards(2, c.GetUsedByPlayer(), row: "back");
        return true;
    }

    internal static bool KraussEffect(Card arg)
    {
        return true;
    }

    internal static bool NatsuhiEffect(Card c)
    {
        GameObject playerField = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.GameObject.transform.parent.parent.name == playerField.name && co.card.Tags.Contains(TagType.Human))
            {
                Game.INSTANCE.AddCounterEffect(co, 2);
            }
        }
        return true;
    }

    internal static bool BernkastelEffect(Card c)
    {
        GameObject field = null;
        if (c.GetUsedByPlayer())
        {
            field = Game.INSTANCE.EnemyField;
        }
        else
        {
            field = Game.INSTANCE.PlayerField;
        }
        GameObject enemyOpenSlot = FindOpenSlot(field);
        if (enemyOpenSlot == null)
        {
            return false;
        }
        while (enemyOpenSlot != null)
        {
            Game.INSTANCE.CreateCardInSlot(Deck.CardsByID.Cats.ToString(), enemyOpenSlot);
            enemyOpenSlot = FindOpenSlot(field);

        }
        Game.INSTANCE.UpdateAllStatBoxes();
        Game.INSTANCE.RecalculateCosts();
        return true;
    }

    internal static bool AngeBeatriceEffect(Card c)
    {
        GameObject playerField = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.GameObject.transform.parent.parent.name != playerField.name)
            {
                if (Game.INSTANCE.GetCounterPanel(co.GameObject) != null)
                {
                    Game.INSTANCE.RemoveCounter(co, co.counters);
                }
            }
        }
        return true;
    }

    internal static bool KanonEffect(Card c)
    {
        CardObject kanon = c.GetTargetCardObject();
        Game.INSTANCE.AddCounterEffect(kanon, 1);
        return true;
    }

    internal static bool RosaEffect(Card arg)
    {
        return true;
    }

    internal static bool SatanEffect(Card c)
    {
        CardObject satanCO = c.GetTargetCardObject();
        Game.INSTANCE.AddCounterEffect(satanCO, 1);
        return true;
    }

    internal static bool PieceEffect(Card c)
    {
        GameObject field = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.GameObject.transform.parent.parent.name != field.name && !co.card.Tags.Contains(TagType.Leader))
            {
                Game.INSTANCE.DamageCard(co, co.currentHP);
                return true;
            }
        }
        return false;
    }

    internal static bool CatsEffect(Card c)
    {
        GameObject field = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.GameObject.transform.parent.parent.name == field.name && co.card.Tags.Contains(TagType.Leader))
            {
                Game.INSTANCE.DamageCard(co, 3);
                return true;
            }
        }
        return false;
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

    internal static bool MariaBeatriceEffect(Card c)
    {
        GameObject field = null;
        if (c.GetUsedByPlayer())
        {
            field = Game.INSTANCE.PlayerField;
        }
        else
        {
            field = Game.INSTANCE.EnemyField;
        }
        foreach(CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if(co.GameObject.transform.parent.parent.name != field.name)
            {
                if (Game.INSTANCE.HasShield(co.GameObject))
                {
                    Game.INSTANCE.RemoveShield(co.GameObject);
                }
                if (Game.INSTANCE.GetCounterPanel(co.GameObject) != null)
                {
                    Game.INSTANCE.RemoveCounter(co, co.counters);
                }
            }
        }
        return true;
    }

    internal static bool RonoveEffect(Card c)
    {
        Game.INSTANCE.CreateShield(c.GetTargetCardObject().GameObject);
        return true;
    }

    internal static bool WillEffect(Card c)
    {
        List<TagType> tags = c.GetTargetCardTags();
        CardObject effectUser = c.GetTargetCardObject();
        if (tags.Contains(TagType.Witch) && effectUser.GameObject.transform.parent.parent.name == "PlayerField")
        {
            return false;
        }
        return true;
    }

    internal static bool SakutarouEffect(Card c)
    {
        GameObject field = null;
        if (c.GetUsedByPlayer())
        {
            field = Game.INSTANCE.PlayerField;
        }
        else
        {
            field = Game.INSTANCE.EnemyField;
        }
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.card.Tags.Contains(TagType.Stake) && field.name == co.GameObject.transform.parent.parent.name)
            {
                Game.INSTANCE.AddCounterEffect(co, 3);
            }
        }
        return true;
    }

    internal static bool MariaEffect(Card c)
    {
        GameObject field = null;
        if (c.GetUsedByPlayer())
        {
            field = Game.INSTANCE.PlayerField;
        }
        else
        {
            field = Game.INSTANCE.EnemyField;
        }
        CardObject sakutarouCO = null;
        foreach(CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if(co.card.ImageName == "Sakutarou" && field.name == co.GameObject.transform.parent.parent.name)
            {
                sakutarouCO = co;
            }
        }
        if(sakutarouCO == null)
        {
            return false;
        }
        if(sakutarouCO.counters == 0)
        {
            return false;
        }
        Game.INSTANCE.AddCounterEffect(sakutarouCO, sakutarouCO.counters);
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
        Game.INSTANCE.AddCounterEffect(willco, 2);
        return true;
    }

    internal static bool KonpeitouEffect(Card c)
    {
        Debug.Log("");
        return true;
    }

    internal static bool LambdaEffect(Card c)
    {
        GameObject gameSlot = Game.INSTANCE.FindFreeSlot(!c.GetUsedByPlayer());
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


    internal static bool ErikaEffect(Card c)
    {
        string fieldName = GetPlayerField(c).name;
        CardObject effectUser = c.GetTargetCardObject();
        string rowCoord = effectUser.GameObject.transform.parent.name.Substring(8, 1);
        if ((Game.INSTANCE.PlayerField.name == fieldName && rowCoord == "1") ||
            (Game.INSTANCE.EnemyField.name == fieldName && rowCoord == "2"))
        {
            return false;
        }
        return true;
    }

    internal static bool BelphegorEffect(Card c)
    {
        CardObject targetCardObject = c.GetTargetCardObject();
        if (!targetCardObject.acted)
        {
            targetCardObject.currentHP = targetCardObject.card.HP + targetCardObject.counters;
            Game.INSTANCE.UpdateAllStatBoxes();
            return true;
        }
        else
        {
            return false;
        }
    }

    internal static bool FeatherineEffect(Card c)
    {
        GameObject field = GetPlayerField(c);
        GameObject os = FindOpenSlot(field);
        if (os == null)
        {
            return false;
        }
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.GameObject.transform.parent.parent.name != field.name && !co.card.Tags.Contains(TagType.Leader))
            {
                co.GameObject.transform.SetParent(os.transform);
                return true;
            }
        }
        return false;
    }

    internal static bool EvaBeatrice2Effect(Card c)
    {
        GameObject field = GetPlayerField(c);
        foreach (CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if (co.GameObject.transform.parent.parent.name != field.name)
            {
                if (Game.INSTANCE.HasShield(co.GameObject))
                {
                    Game.INSTANCE.RemoveShield(co.GameObject);
                }
            }
        }
        return true;
    }

    internal static bool EvaBeatriceEffect(Card c)
    {
        CardObject cardMoving = c.GetTargetCardObject();
        GameObject field = GetPlayerField(c);
        foreach(CardObject co in Game.INSTANCE.CardObjectsInGame)
        {
            if(co.card.ImageName == "Dlanor" && co.GameObject.transform.parent.parent.name != field.name)
            {
                return true;
            }
        }
        if (cardMoving.card.Cost == 1 && cardMoving.GameObject.transform.parent.parent.name != field.name)
        {
            return false;
        }
        return true;
    }

    internal static GameObject FindOpenSlot(GameObject field)
    {
        for(int i=0; i<field.transform.childCount; i++)
        {
            GameObject slot = field.transform.GetChild(i).gameObject;
            if (!Game.INSTANCE.SlotWithCard(slot))
            {
                return slot;
            }
        }
        return null;
    }
}
