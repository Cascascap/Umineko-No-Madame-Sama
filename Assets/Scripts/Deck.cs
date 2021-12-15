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
        Summon,
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

    public void StartingDeck()
    {
        this.leaderCard = new Card(0, 36, 3, CardEffects.BeatriceEffect, "Beatrice", new List<TagType> { TagType.Leader, TagType.Witch }, false, false, 1);
        AddCardRegister(this.leaderCard, true);
        Card goat = new Card(1, 2, 2, CardEffects.GoatEffect, "Goat", new List<TagType> { TagType.Summon }, true, false, 0);
        AddCardRegister(goat, times: 10);
        Card asmodeus = new Card(1, 4, 4, CardEffects.AsmodeusEffect, "Asmodeus", new List<TagType> { TagType.Summon, TagType.Stake }, false, true, 1, TagType.Summon, TargetType.Ally);
        AddCardRegister(asmodeus);
        Card beelzebub = new Card(1, 5, 3, CardEffects.BeelzebubEffect, "Beelzebub", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        AddCardRegister(beelzebub);
        EffectListener.INSTANCE.GettingCountersList.Add(beelzebub);
        Card belphegor = new Card(1, 6, 2, CardEffects.BelphegorEffect, "Belphegor", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        EffectListener.INSTANCE.TurnEndingList.Add(belphegor);
        AddCardRegister(belphegor);
        AddCardRegister(new Card(1, 4, 4, CardEffects.LeviathanEffect, "Leviathan", new List<TagType> { TagType.Summon, TagType.Stake }, false, true, 1, TagType.All, TargetType.Both));
        AddCardRegister(new Card(1, 4, 4, CardEffects.MammonEffect, "Mammon", new List<TagType> { TagType.Summon, TagType.Stake }, false, false, 2));
        Card satan = new Card(1, 2, 6, CardEffects.SatanEffect, "Satan", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        AddCardRegister(satan);
        EffectListener.INSTANCE.DestroysCardsList.Add(satan);
        Card lucifer = new Card(2, 7, 7, CardEffects.LuciferEffect, "Lucifer", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        AddCardRegister(lucifer);
        EffectListener.INSTANCE.CardPlayedList.Add(lucifer);
        Card gaap = new Card(2, 4, 4, CardEffects.GaapEffect, "Gaap", new List<TagType> { TagType.Summon, TagType.Demon }, true, false, 0);
        AddCardRegister(gaap);
        EffectListener.INSTANCE.CanAttackFromAnywhereList.Add(gaap);
        AddCardRegister(new Card(2, 8, 4, CardEffects.RonoveEffect, "Ronove", new List<TagType> { TagType.Summon, TagType.Demon }, false, true, 2, TagType.All, TargetType.Ally));
        AddCardRegister(new Card(3, 12, 6, CardEffects.VirgiliaEffect, "Virgilia", new List<TagType> { TagType.Witch }, false, false, 3));
    }

    //Default decks:
    public void Beatrice()
    {
        StartingDeck();
    }

    public void Lambda()
    {
        this.leaderCard = new Card(0, 36, 3, CardEffects.LambdaEffect, "Lambda", new List<TagType> { TagType.Leader, TagType.Witch}, false, false, 1);
        AddCardRegister(this.leaderCard, true);
        Card Konpeitou = new Card(1, 1, 3, CardEffects.KonpeitouEffect, "Konpeitou", new List<TagType> { TagType.Summon, TagType.Object }, true, false, 0);
        AddCardRegister(Konpeitou, times:4);
        AddCardRegister(new Card(2, 6, 2, CardEffects.LionEffect, "Lion", new List<TagType> { TagType.Human }, false, false, 1, requiresAI:true));
        Card will = new Card(3, 8, 10, CardEffects.WillEffect, "Will", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(will);
        EffectListener.INSTANCE.EffectStopperList.Add(will);
        Card diana = new Card(1, 2, 2, CardEffects.DianaEffect, "Diana", new List<TagType> { TagType.Cat, TagType.Pet }, true, false, 1);
        EffectListener.INSTANCE.TurnEndingList.Add(diana);
        AddCardRegister(diana);
    }

    public void Kinzo()
    {
        this.leaderCard = new Card(0, 30, 2, CardEffects.KinzoEffect, "Kinzo", new List<TagType> { TagType.Leader, TagType.Human }, false, true, 1, TagType.Servant, TargetType.Ally);
        AddCardRegister(this.leaderCard, true);
        Card Genji = new Card(2, 12, 2, CardEffects.GenjiEffect, "Genji", new List<TagType> { TagType.Servant, TagType.Human }, true, false, 0);
        EffectListener.INSTANCE.TurnEndingList.Add(Genji);
        AddCardRegister(Genji);
        Card Gohda = new Card(2, 10, 4, CardEffects.GohdaEffect, "Gohda", new List<TagType> { TagType.Human, TagType.Servant }, true, false, 1);
        EffectListener.INSTANCE.TurnEndingList.Add(Gohda);
        AddCardRegister(Gohda);
        Card Kanon = new Card(1, 6, 6, CardEffects.KanonEffect, "Kanon", new List<TagType> { TagType.Human, TagType.Servant }, true, false, 0);
        AddCardRegister(Kanon);
        EffectListener.INSTANCE.DestroysCardsList.Add(Kanon);
        Card Kumasawa = new Card(1, 2, 2, CardEffects.KumasawaEffect, "Kumasawa", new List<TagType> { TagType.Human, TagType.Servant }, false, false, 2);
        AddCardRegister(Kumasawa);
        AddCardRegister(new Card(2, 10, 4, CardEffects.NanjoEffect, "Nanjo", new List<TagType> { TagType.Human }, false, true, 1, requiresAI: true));
        AddCardRegister(new Card(1, 2, 2, CardEffects.ShannonEffect, "Shannon", new List<TagType> { TagType.Human, TagType.Servant }, false, true, 1, requiresAI: true));
    }

    public void Battler()
    {
        this.leaderCard = new Card(0, 40, 1, CardEffects.BattlerEffect, "Battler", new List<TagType> { TagType.Leader, TagType.Human }, false, false, 1);
        AddCardRegister(this.leaderCard, true);
        Card Rudolf = new Card(1, 10, 1, CardEffects.RudolfEffect, "Rudolf", new List<TagType> { TagType.Human }, true, false, 0);
        EffectListener.INSTANCE.TurnEndingList.Add(Rudolf);
        AddCardRegister(Rudolf);
        Card Kyrie = new Card(1, 10, 1, CardEffects.KyrieEffect, "Kyrie", new List<TagType> { TagType.Human }, true, false, 0);
        EffectListener.INSTANCE.TurnEndingList.Add(Kyrie);
        AddCardRegister(Kyrie);
        Card Eva = new Card(1, 9, 3, CardEffects.EvaEffect, "Eva", new List<TagType> { TagType.Human }, true, false, 0);
        EffectListener.INSTANCE.TurnEndingList.Add(Eva);
        AddCardRegister(Eva);
        Card George = new Card(1, 4, 8, CardEffects.GeorgeEffect, "George", new List<TagType> { TagType.Human }, false, true, 0);
        AddCardRegister(George);
        Card Hideyoshi = new Card(1, 10, 2, CardEffects.HideyoshiEffect, "Hideyoshi", new List<TagType> { TagType.Human }, true, false, 0);
        EffectListener.INSTANCE.CanAttackFromAnywhereList.Add(Hideyoshi);
        AddCardRegister(Hideyoshi);
        Card Jessica = new Card(1, 5, 8, CardEffects.JessicaEffect, "Jessica", new List<TagType> { TagType.Human }, false, false, 0);
        AddCardRegister(Jessica);
        Card Krauss = new Card(1, 8, 8, CardEffects.KraussEffect, "Krauss", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(Krauss);
        Card Natsuhi = new Card(1, 8, 8, CardEffects.NatsuhiEffect, "Natsuhi", new List<TagType> { TagType.Human }, true, false, 0);
        EffectListener.INSTANCE.TurnEndingList.Add(Natsuhi);
        AddCardRegister(Natsuhi);
        Card Rosa = new Card(2, 20, 6, CardEffects.RosaEffect, "Rosa", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(Rosa);
        EffectListener.INSTANCE.CanAttackFromAnywhereList.Add(Rosa);
    }

    public void Ange()
    {
        this.leaderCard = new Card(0, 36, 3, CardEffects.AngeEffect, "Ange", new List<TagType> { TagType.Leader, TagType.Human }, false, false, 1);
        AddCardRegister(this.leaderCard, true);
        Card asmodeus = new Card(1, 4, 4, CardEffects.AsmodeusEffect, "Asmodeus", new List<TagType> { TagType.Summon, TagType.Stake }, false, true, 1, TagType.Summon, TargetType.Ally);
        AddCardRegister(asmodeus);
        Card beelzebub = new Card(1, 5, 3, CardEffects.BeelzebubEffect, "Beelzebub", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        AddCardRegister(beelzebub);
        EffectListener.INSTANCE.GettingCountersList.Add(beelzebub);
        Card belphegor = new Card(1, 6, 2, CardEffects.BelphegorEffect, "Belphegor", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        EffectListener.INSTANCE.TurnEndingList.Add(belphegor);
        AddCardRegister(belphegor);
        AddCardRegister(new Card(1, 4, 4, CardEffects.LeviathanEffect, "Leviathan", new List<TagType> { TagType.Summon, TagType.Stake }, false, true, 1, TagType.All, TargetType.Both));
        AddCardRegister(new Card(1, 4, 4, CardEffects.MammonEffect, "Mammon", new List<TagType> { TagType.Summon, TagType.Stake }, false, false, 2));
        Card satan = new Card(1, 2, 6, CardEffects.SatanEffect, "Satan", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        AddCardRegister(satan);
        EffectListener.INSTANCE.DestroysCardsList.Add(satan);
        Card lucifer = new Card(2, 7, 7, CardEffects.LuciferEffect, "Lucifer", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        AddCardRegister(lucifer);
        EffectListener.INSTANCE.CardPlayedList.Add(lucifer);
        Card Sakutarou = new Card(1, 5, 1, CardEffects.SakutarouEffect, "Sakutarou", new List<TagType> { TagType.Summon}, true, false, 0);
        EffectListener.INSTANCE.TurnEndingList.Add(Sakutarou);
        AddCardRegister(Sakutarou);
        Card Maria = new Card(2, 4, 1, CardEffects.MariaEffect, "Maria", new List<TagType> { TagType.Human}, true, false, 0);
        EffectListener.INSTANCE.TurnEndingList.Add(Maria);
        AddCardRegister(Maria);
        Card MariaBeatrice = new Card(3, 8, 6, CardEffects.MariaBeatriceEffect, "MariaBeatrice", new List<TagType> { TagType.Witch }, false, false, 0);
        AddCardRegister(MariaBeatrice);
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
