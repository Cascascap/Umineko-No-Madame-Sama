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
    private void AddCardRegister(Card card, bool leader = false)
    {
        if (Game.INSTANCE != null)
        {
            Game.INSTANCE.CardsInGame.Add(card);
        }
        if (!leader)
        {
            this.cards.Push(card);
        }
    }

    public void GoatCard()
    {
        Card goat = new Card(1, 2, 2, CardEffects.GoatEffect, "Goat", new List<TagType> { TagType.Summon }, true, false, 0);
        AddCardRegister(goat);
    }

    public void AsmodeusCard()
    {
        Card asmodeus = new Card(1, 4, 4, CardEffects.AsmodeusEffect, "Asmodeus", new List<TagType> { TagType.Summon, TagType.Stake }, false, true, 1, TagType.Summon, TargetType.Ally);
        AddCardRegister(asmodeus);
    }

    public void BeelzebubCard()
    {
        Card beelzebub = new Card(1, 5, 3, CardEffects.BeelzebubEffect, "Beelzebub", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        AddCardRegister(beelzebub);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.GettingCountersList.Add(beelzebub);
        }
    }

    public void BelphegorCard()
    {
        Card belphegor = new Card(1, 6, 2, CardEffects.BelphegorEffect, "Belphegor", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        AddCardRegister(belphegor);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(belphegor);
        }
    }

    public void LeviathanCard()
    {
        AddCardRegister(new Card(1, 4, 4, CardEffects.LeviathanEffect, "Leviathan", new List<TagType> { TagType.Summon, TagType.Stake }, false, true, 1, TagType.All, TargetType.Both));
    }

    public void MammonCard()
    {
        AddCardRegister(new Card(1, 4, 4, CardEffects.MammonEffect, "Mammon", new List<TagType> { TagType.Summon, TagType.Stake }, false, false, 2));
    }

    public void SatanCard()
    {
        Card satan = new Card(1, 2, 6, CardEffects.SatanEffect, "Satan", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        AddCardRegister(satan);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.DestroysCardsList.Add(satan);
        }
    }

    public void LuciferCard()
    {
        Card lucifer = new Card(2, 7, 7, CardEffects.LuciferEffect, "Lucifer", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0);
        AddCardRegister(lucifer);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.CardPlayedList.Add(lucifer);
        }
    }

    public void GaapCard()
    {
        Card gaap = new Card(2, 4, 4, CardEffects.GaapEffect, "Gaap", new List<TagType> { TagType.Summon, TagType.Demon }, true, false, 0);
        AddCardRegister(gaap);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.CanAttackFromAnywhereList.Add(gaap);
        }
    }

    public void RonoveCard()
    {
        AddCardRegister(new Card(2, 8, 4, CardEffects.RonoveEffect, "Ronove", new List<TagType> { TagType.Summon, TagType.Demon }, false, true, 2, TagType.All, TargetType.Ally));
    }

    public void VirgiliaCard()
    {
        AddCardRegister(new Card(3, 12, 6, CardEffects.VirgiliaEffect, "Virgilia", new List<TagType> { TagType.Witch }, false, false, 3));
    }

    public void KonpeitouCard()
    {
        Card Konpeitou = new Card(1, 1, 3, CardEffects.KonpeitouEffect, "Konpeitou", new List<TagType> { TagType.Summon, TagType.Object }, true, false, 0);
        AddCardRegister(Konpeitou);
    }
    public void LionCard()
    {
        AddCardRegister(new Card(2, 6, 2, CardEffects.LionEffect, "Lion", new List<TagType> { TagType.Human}, false, false, 1, requiresAI: true));
    }

    public void WillCard()
    {
        Card will = new Card(3, 8, 10, CardEffects.WillEffect, "Will", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(will);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.EffectStopperList.Add(will);
        }
    }
    public void DianaCard()
    {
        Card diana = new Card(1, 2, 2, CardEffects.DianaEffect, "Diana", new List<TagType> { TagType.Cat, TagType.Pet }, true, false, 1);
        AddCardRegister(diana);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(diana);
        }
    }

    public void GenjiCard()
    {
        Card Genji = new Card(2, 12, 2, CardEffects.GenjiEffect, "Genji", new List<TagType> { TagType.Servant, TagType.Human }, true, false, 0);
        AddCardRegister(Genji);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(Genji);
        }
    }

    public void GohdaCard()
    {
        Card Gohda = new Card(2, 10, 4, CardEffects.GohdaEffect, "Gohda", new List<TagType> { TagType.Human, TagType.Servant }, true, false, 1);
        AddCardRegister(Gohda);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(Gohda);
        }
    }

    public void KanonCard()
    {
        Card Kanon = new Card(1, 6, 6, CardEffects.KanonEffect, "Kanon", new List<TagType> { TagType.Human, TagType.Servant }, true, false, 0);
        AddCardRegister(Kanon);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.DestroysCardsList.Add(Kanon);
        }
    }

    public void KumasawaCard()
    {
        Card Kumasawa = new Card(1, 2, 2, CardEffects.KumasawaEffect, "Kumasawa", new List<TagType> { TagType.Human, TagType.Servant }, false, false, 2);
        AddCardRegister(Kumasawa);
    }
    public void NanjoCard()
    {
        AddCardRegister(new Card(2, 10, 4, CardEffects.NanjoEffect, "Nanjo", new List<TagType> { TagType.Human}, false, true, 1, requiresAI: true));
    }
    public void ShannonCard()
    {
        AddCardRegister(new Card(1, 2, 2, CardEffects.ShannonEffect, "Shannon", new List<TagType> { TagType.Human, TagType.Servant }, false, true, 1, requiresAI: true));
    }

    public void RudolfCard()
    {
        Card Rudolf = new Card(1, 10, 1, CardEffects.RudolfEffect, "Rudolf", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(Rudolf);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(Rudolf);
        }
    }
    public void KyrieCard()
    {
        Card Kyrie = new Card(1, 10, 1, CardEffects.KyrieEffect, "Kyrie", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(Kyrie);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(Kyrie);
        }
    }

    public void EvaCard()
    {
        Card Eva = new Card(1, 9, 3, CardEffects.EvaEffect, "Eva", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(Eva);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(Eva);
        }
    }
    public void GeorgeCard()
    {
        Card George = new Card(1, 4, 8, CardEffects.GeorgeEffect, "George", new List<TagType> { TagType.Human }, false, true, 0);
        AddCardRegister(George);
    }
    public void HideyoshiCard()
    {
        Card Hideyoshi = new Card(1, 10, 2, CardEffects.HideyoshiEffect, "Hideyoshi", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(Hideyoshi);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.CanAttackFromAnywhereList.Add(Hideyoshi);
        }
    }
    public void JessicaCard()
    {
        Card Jessica = new Card(1, 5, 8, CardEffects.JessicaEffect, "Jessica", new List<TagType> { TagType.Human }, false, false, 0);
        AddCardRegister(Jessica);
    }
    public void KraussCard()
    {
        Card Krauss = new Card(1, 8, 8, CardEffects.KraussEffect, "Krauss", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(Krauss);
    }
    public void NatsuhiCard()
    {
        Card Natsuhi = new Card(1, 8, 8, CardEffects.NatsuhiEffect, "Natsuhi", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(Natsuhi);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(Natsuhi);
        }
    }
    public void RosaCard()
    {
        Card Rosa = new Card(2, 20, 6, CardEffects.RosaEffect, "Rosa", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(Rosa);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.CanAttackFromAnywhereList.Add(Rosa);
        }
    }

    public void SakutarouCard()
    {
        Card Sakutarou = new Card(1, 5, 1, CardEffects.SakutarouEffect, "Sakutarou", new List<TagType> { TagType.Summon }, true, false, 0);
        AddCardRegister(Sakutarou);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(Sakutarou);
        }
    }

    public void MariaCard()
    {
        Card Maria = new Card(2, 4, 1, CardEffects.MariaEffect, "Maria", new List<TagType> { TagType.Human }, true, false, 0);
        AddCardRegister(Maria);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(Maria);
        }
    }
    public void MariaBeatriceCard()
    {
        Card MariaBeatrice = new Card(3, 8, 6, CardEffects.MariaBeatriceEffect, "MariaBeatrice", new List<TagType> { TagType.Witch }, false, false, 0);
        AddCardRegister(MariaBeatrice);
    }
    

    public void StartingDeck()
    {
        AddCardToDeck(CardsByID.Goat, 10);
        AddCardToDeck(CardsByID.Asmodeus);
        AddCardToDeck(CardsByID.Beelzebub);
        AddCardToDeck(CardsByID.Belphegor);
        AddCardToDeck(CardsByID.Leviathan);
        AddCardToDeck(CardsByID.Mammon);
        AddCardToDeck(CardsByID.Satan);
        AddCardToDeck(CardsByID.Lucifer);
        AddCardToDeck(CardsByID.Gaap);
        AddCardToDeck(CardsByID.Ronove);
        AddCardToDeck(CardsByID.Virgilia);
        PlayerPrefs.SetInt("PlayerHasDeck", 1);
    }

    private void SaveCardToDeck(CardsByID goat, int cardNumber)
    {
        PlayerPrefs.SetInt(goat.ToString(), cardNumber);
    }


    public void Beatrice()
    {
        this.leaderCard = new Card(0, 36, 3, CardEffects.BeatriceEffect, "Beatrice", new List<TagType> { TagType.Leader, TagType.Witch }, false, false, 1);
        AddCardRegister(this.leaderCard, true);
        if (PlayerPrefs.GetInt("PlayerHasDeck") != 1)
        {
            StartingDeck();
        }
        else
        {
            LoadDeck();
        }
    }

    public void LoadDeck()
    {
        foreach(CardsByID cbi in Enum.GetValues(typeof(CardsByID)))
        {
            int numberOfCardsInDeck = PlayerPrefs.GetInt(cbi.ToString());
            if(numberOfCardsInDeck > 0)
            {
                AddCardToDeck(cbi, numberOfCardsInDeck, false);
            }
        }
    }

    public void RemoveCardFromDeck(CardsByID card)
    {
        Stack<Card> newCards = new Stack<Card>();
        bool removed = false;
        foreach(Card c in cards)
        {
            if(c.ImageName == card.ToString() && !removed)
            {
                continue;
            }
            else
            {
               
            }
        }
    }

    public void AddCardToDeck(CardsByID card, int cardNumber=1, bool saveCard=true, bool addToinventory=false)
    {
        for(int i=0; i<cardNumber; i++)
        {
            CallMethod(card.ToString() + "Card");
        }
        if (saveCard)
        {
            SaveCardToDeck(card, cardNumber);
        }
        if (addToinventory)
        {
            AddCardToInventory(card);
        }
    }

    private void AddCardToInventory(CardsByID card, int number=1)
    {
        int inInventory = PlayerPrefs.GetInt(card.ToString() + "Inventory");
        PlayerPrefs.SetInt(card.ToString() + "Inventory", inInventory + number);
    }

    public void Lambda()
    {
        this.leaderCard = new Card(0, 36, 3, CardEffects.LambdaEffect, "Lambda", new List<TagType> { TagType.Leader, TagType.Witch}, false, false, 1);
        AddCardRegister(this.leaderCard, true);

        AddCardToDeck(CardsByID.Konpeitou, 4, false);
        AddCardToDeck(CardsByID.Lion, saveCard:false);
        AddCardToDeck(CardsByID.Will, saveCard: false);
        AddCardToDeck(CardsByID.Diana, saveCard: false);
    }

    public void Kinzo()
    {
        this.leaderCard = new Card(0, 30, 2, CardEffects.KinzoEffect, "Kinzo", new List<TagType> { TagType.Leader, TagType.Human }, false, true, 1, TagType.Servant, TargetType.Ally);
        AddCardRegister(this.leaderCard, true);


        AddCardToDeck(CardsByID.Genji, saveCard: false);
        AddCardToDeck(CardsByID.Gohda, saveCard: false);
        AddCardToDeck(CardsByID.Kanon, saveCard: false);
        AddCardToDeck(CardsByID.Kumasawa, saveCard: false);
        AddCardToDeck(CardsByID.Nanjo, saveCard: false);
        AddCardToDeck(CardsByID.Shannon, saveCard: false);
    }
    public void Ange()
    {
        this.leaderCard = new Card(0, 36, 3, CardEffects.AngeEffect, "Ange", new List<TagType> { TagType.Leader, TagType.Human }, false, false, 1);
        AddCardRegister(this.leaderCard, true);

        AddCardToDeck(CardsByID.Asmodeus, saveCard: false);
        AddCardToDeck(CardsByID.Beelzebub, saveCard: false);
        AddCardToDeck(CardsByID.Belphegor, saveCard: false);
        AddCardToDeck(CardsByID.Leviathan, saveCard: false);
        AddCardToDeck(CardsByID.Mammon, saveCard: false);
        AddCardToDeck(CardsByID.Satan, saveCard: false);
        AddCardToDeck(CardsByID.Lucifer, saveCard: false);
        AddCardToDeck(CardsByID.Sakutarou, saveCard: false);
        AddCardToDeck(CardsByID.Maria, saveCard: false);
        AddCardToDeck(CardsByID.MariaBeatrice, saveCard: false);
    }

    public void Battler()
    {
        this.leaderCard = new Card(0, 40, 1, CardEffects.BattlerEffect, "Battler", new List<TagType> { TagType.Leader, TagType.Human }, false, false, 1);
        AddCardRegister(this.leaderCard, true);

        AddCardToDeck(CardsByID.Rudolf, saveCard: false);
        AddCardToDeck(CardsByID.Kyrie, saveCard: false);
        AddCardToDeck(CardsByID.Eva, saveCard: false);
        AddCardToDeck(CardsByID.George, saveCard: false);
        AddCardToDeck(CardsByID.Hideyoshi, saveCard: false);
        AddCardToDeck(CardsByID.Jessica, saveCard: false);
        AddCardToDeck(CardsByID.Krauss, saveCard: false);
        AddCardToDeck(CardsByID.Natsuhi, saveCard: false);
        AddCardToDeck(CardsByID.Rosa, saveCard: false);
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


    public enum CardsByID
    {
        Goat=1,
        Lucifer=2,
        Leviathan=3,
        Satan=4,
        Belphegor=5,
        Mammon=6,
        Beelzebub=7,
        Asmodeus=8,
        Gaap=9,
        Ronove=10,
        Virgilia=11,
        Genji=12,
        Kumasawa=13,
        Gohda=14,
        Kanon=15,
        Shannon=16,
        Nanjo=17,
        Maria=18,
        MariaBeatrice=19,
        Sakutarou=20,
        Rudolf=21,
        Kyrie=22,
        Krauss=23,
        Natsuhi=24,
        Eva=25,
        Hideyoshi=26,
        Rosa=27,
        George=28,
        Jessica=29,
        Chiester00=30,
        Chiester410=31,
        Chiester45=32,
        Chiester556=33,
        Dlanor=34,
        Gertrude=35,
        Cornelia=36,
        Will=37,
        Lion=38,
        Diana=39,
        Konpeitou=40,
        Bernkastel=41,
        Cat=42,
        Piece=43,
        AngeBeatrice=44,
        EvaBeatrice=45
    }

    public static List<Card> GetAllCards()
    {
        List<Card> ret = new List<Card>
        {
        new Card(1, 2, 2, CardEffects.GoatEffect, "Goat", new List<TagType> { TagType.Summon }, true, false, 0),
        new Card(2, 7, 7, CardEffects.LuciferEffect, "Lucifer", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0),
        new Card(1, 4, 4, CardEffects.LeviathanEffect, "Leviathan", new List<TagType> { TagType.Summon, TagType.Stake }, false, true, 1, TagType.All, TargetType.Both),
        new Card(1, 2, 6, CardEffects.SatanEffect, "Satan", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0),
        new Card(1, 6, 2, CardEffects.BelphegorEffect, "Belphegor", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0),
        new Card(1, 4, 4, CardEffects.MammonEffect, "Mammon", new List<TagType> { TagType.Summon, TagType.Stake }, false, false, 2),
        new Card(1, 5, 3, CardEffects.BeelzebubEffect, "Beelzebub", new List<TagType> { TagType.Summon, TagType.Stake }, true, false, 0),
        new Card(1, 4, 4, CardEffects.AsmodeusEffect, "Asmodeus", new List<TagType> { TagType.Summon, TagType.Stake }, false, true, 1, TagType.Summon, TargetType.Ally),
        new Card(2, 4, 4, CardEffects.GaapEffect, "Gaap", new List<TagType> { TagType.Summon, TagType.Demon }, true, false, 0),
        new Card(2, 8, 4, CardEffects.RonoveEffect, "Ronove", new List<TagType> { TagType.Summon, TagType.Demon }, false, true, 2, TagType.All, TargetType.Ally),
        new Card(3, 12, 6, CardEffects.VirgiliaEffect, "Virgilia", new List<TagType> { TagType.Witch }, false, false, 3),
        new Card(2, 12, 2, CardEffects.GenjiEffect, "Genji", new List<TagType> { TagType.Servant, TagType.Human }, true, false, 0),
        new Card(1, 2, 2, CardEffects.KumasawaEffect, "Kumasawa", new List<TagType> { TagType.Human, TagType.Servant }, false, false, 2),
        new Card(2, 10, 4, CardEffects.GohdaEffect, "Gohda", new List<TagType> { TagType.Human, TagType.Servant }, true, false, 1),
        new Card(1, 6, 6, CardEffects.KanonEffect, "Kanon", new List<TagType> { TagType.Human, TagType.Servant }, true, false, 0),
        new Card(1, 2, 2, CardEffects.ShannonEffect, "Shannon", new List<TagType> { TagType.Human, TagType.Servant }, false, true, 1, requiresAI: true),
        new Card(2, 10, 4, CardEffects.NanjoEffect, "Nanjo", new List<TagType> { TagType.Human }, false, true, 1, requiresAI: true),
        new Card(2, 4, 1, CardEffects.MariaEffect, "Maria", new List<TagType> { TagType.Human }, true, false, 0),
        new Card(3, 8, 6, CardEffects.MariaBeatriceEffect, "MariaBeatrice", new List<TagType> { TagType.Witch }, false, false, 0),
        new Card(1, 5, 1, CardEffects.SakutarouEffect, "Sakutarou", new List<TagType> { TagType.Summon }, true, false, 0),
        new Card(1, 10, 1, CardEffects.RudolfEffect, "Rudolf", new List<TagType> { TagType.Human }, true, false, 0),
        new Card(1, 10, 1, CardEffects.KyrieEffect, "Kyrie", new List<TagType> { TagType.Human }, true, false, 0),
        new Card(1, 9, 3, CardEffects.EvaEffect, "Eva", new List<TagType> { TagType.Human }, true, false, 0),
        new Card(1, 4, 8, CardEffects.GeorgeEffect, "George", new List<TagType> { TagType.Human }, false, true, 0),
        new Card(1, 10, 2, CardEffects.HideyoshiEffect, "Hideyoshi", new List<TagType> { TagType.Human }, true, false, 0),
        new Card(1, 5, 8, CardEffects.JessicaEffect, "Jessica", new List<TagType> { TagType.Human }, false, false, 0),
        new Card(1, 8, 8, CardEffects.KraussEffect, "Krauss", new List<TagType> { TagType.Human }, true, false, 0),
        new Card(1, 8, 8, CardEffects.NatsuhiEffect, "Natsuhi", new List<TagType> { TagType.Human }, true, false, 0),
        new Card(2, 20, 6, CardEffects.RosaEffect, "Rosa", new List<TagType> { TagType.Human }, true, false, 0)
        };
        return ret;
    }
}
