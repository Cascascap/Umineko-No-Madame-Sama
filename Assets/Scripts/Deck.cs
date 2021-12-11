using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Deck 
{
    public enum TargetType
    {
        Ally,
        Enemy,
        Both
    }

    public enum TagType
    {
        All,
        Leader,
        Witch,
        Human,
        Demon,
        Stake,
        Summoned,
        Cat,
        Pet,
        Object,
        Servant
    }

    public int remainingCards = 0;
    public Stack<Card> cards = new Stack<Card>();
    public Card leaderCard;

    public void InitializeDeck(string leaderName)
    {
        this.cards = new Stack<Card>();
        CallMethod(leaderName);
        this.remainingCards = cards.Count;
        Debug.Log(this.remainingCards);
    }

    public Stack<Card> CallMethod(string method)
    {
        try
        {
            Type type = typeof(Deck);
            MethodInfo methodInfo = type.GetMethod(method);
            return (Stack<Card>) methodInfo.Invoke(this, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.ReadKey();
            return null;
        }
    }

    public void Shuffle()
    {
        System.Random rnd = new System.Random();
        var values = this.cards.ToArray();
        this.cards.Clear();
        foreach (var value in values.OrderBy(x => rnd.Next()))
            this.cards.Push(value);
    }
    private void AddCardRegister(Card card, bool leader = false, int times = 1)
    {
        Game.INSTANCE.CardsInGame.Add(card);
        if (!leader)
        {
            for (int i = 0; i < times; i++)
            {
                this.cards.Push(card);
            }
        }
    }

    //Default decks:
    public void Beatrice()
    {
        this.leaderCard = new Card(0, 36, 3, CardEffects.BeatriceEffect, "Beatrice", new List<TagType> { TagType.Leader, TagType.Witch }, false, false, 1);
        AddCardRegister(this.leaderCard, true);
        Card goat = new Card(1, 2, 2, CardEffects.GoatEffect, "Goat", new List<TagType> { TagType.Summoned }, true, false, 0);
        AddCardRegister(goat, times:10);
        Card asmodeus = new Card(1, 4, 4, CardEffects.AsmodeusEffect, "Asmodeus", new List<TagType> { TagType.Summoned, TagType.Stake }, false, true, 1, TagType.Summoned, TargetType.Ally);
        AddCardRegister(asmodeus);
        Card beelzebub = new Card(1, 5, 3, CardEffects.BeelzebubEffect, "Beelzebub", new List<TagType> { TagType.Summoned, TagType.Stake }, true, false, 0);
        AddCardRegister(beelzebub);
        EffectListener.INSTANCE.GettingCountersList.Add(beelzebub);
        Card belphegor = new Card(1, 6, 2, CardEffects.BelphegorEffect, "Belphegor", new List<TagType> { TagType.Summoned, TagType.Stake }, true, false, 0);
        EffectListener.INSTANCE.TurnEndingList.Add(belphegor);
        AddCardRegister(belphegor);
        AddCardRegister(new Card(1, 4, 4, CardEffects.LeviathanEffect, "Leviathan", new List<TagType> { TagType.Summoned, TagType.Stake }, false, true, 1, TagType.All, TargetType.Both));
        AddCardRegister(new Card(1, 4, 4, CardEffects.MammonEffect, "Mammon", new List<TagType> { TagType.Summoned, TagType.Stake }, false, false, 2));
        Card satan = new Card(1, 2, 6, CardEffects.SatanEffect, "Satan", new List<TagType> { TagType.Summoned, TagType.Stake }, true, false, 0);
        AddCardRegister(satan);
        EffectListener.INSTANCE.DestroysCardsList.Add(satan);
        Card lucifer = new Card(2, 7, 7, CardEffects.LuciferEffect, "Lucifer", new List<TagType> { TagType.Summoned, TagType.Stake }, true, false, 0);
        AddCardRegister(lucifer);
        EffectListener.INSTANCE.CardPlayedList.Add(lucifer);
        Card gaap = new Card(2, 4, 4, CardEffects.GaapEffect, "Gaap", new List<TagType> { TagType.Summoned, TagType.Demon }, true, false, 0);
        AddCardRegister(gaap);
        EffectListener.INSTANCE.CanAttackFromAnywhereList.Add(gaap);
        AddCardRegister(new Card(2, 8, 4, CardEffects.RonoveEffect, "Ronove", new List<TagType> { TagType.Summoned, TagType.Demon }, false, true, 2, TagType.All, TargetType.Ally));
        AddCardRegister(new Card(3, 12, 6, CardEffects.VirgiliaEffect, "Virgilia", new List<TagType> { TagType.Witch }, false, false, 3));
    }

    public void Lambda()
    {
        this.leaderCard = new Card(0, 36, 3, CardEffects.LambdaEffect, "Lambda", new List<TagType> { TagType.Leader, TagType.Witch}, false, false, 1);
        AddCardRegister(this.leaderCard, true);
        Card Konpeitou = new Card(1, 1, 3, CardEffects.KonpeitouEffect, "Konpeitou", new List<TagType> { TagType.Summoned, TagType.Object }, true, false, 0);
        AddCardRegister(Konpeitou, times:4);
        AddCardRegister(new Card(2, 6, 2, CardEffects.LionEffect, "Lion", new List<TagType> { TagType.Human }, false, false, 1));
        Card will = new Card(3, 8, 10, CardEffects.WillEffect, "Will", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(will);
        EffectListener.INSTANCE.EffectStopperList.Add(will);
        Card diana = new Card(1, 2, 2, CardEffects.DianaEffect, "Diana", new List<TagType> { TagType.Cat, TagType.Pet }, true, false, 1);
        EffectListener.INSTANCE.TurnEndingList.Add(diana);
        AddCardRegister(diana);
    }

    public void Kinzo()
    {
        this.leaderCard = new Card(0, 30, 2, CardEffects.KinzoEffect, "Kinzo", new List<TagType> { TagType.Leader, TagType.Human }, false, true, 1);
        AddCardRegister(this.leaderCard, true);
        Card Genji = new Card(2, 12, 2, CardEffects.GenjiEffect, "Genji", new List<TagType> { TagType.Servant, TagType.Human }, true, false, 0);
        EffectListener.INSTANCE.TurnEndingList.Add(Genji);
        AddCardRegister(new Card(2, 10, 4, CardEffects.GohdaEffect, "Gohda", new List<TagType> { TagType.Human, TagType.Servant }, false, false, 1));
        Card Kanon = new Card(1, 6, 6, CardEffects.KanonEffect, "Kanon", new List<TagType> { TagType.Human, TagType.Servant }, true, false, 0);
        AddCardRegister(Kanon);
        EffectListener.INSTANCE.DestroysCardsList.Add(Kanon);
        Card Kumasawa = new Card(1, 2, 2, CardEffects.KumasawaEffect, "Kumasawa", new List<TagType> { TagType.Human, TagType.Servant }, false, false, 2);
        AddCardRegister(Kumasawa);
        AddCardRegister(new Card(2, 10, 4, CardEffects.NanjoEffect, "Nanjo", new List<TagType> { TagType.Human }, false, true, 1));
        AddCardRegister(new Card(1, 2, 2, CardEffects.ShannonEffect, "Shannon", new List<TagType> { TagType.Human, TagType.Servant }, false, true, 1));
    }

    public Card FindCardInDeck(string name)
    {
        List<Card> cardsList = this.cards.ToList(); 
        if(name == leaderCard.ImageName)
        {
            return leaderCard;
        }
        foreach (Card c in cardsList)
        {
            if(c.ImageName == name)
            {
                return c;
            }
        }
        return null;
    }
}
