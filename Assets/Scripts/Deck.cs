using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = System.Random;

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
        Servant,
        Angel,
        Chiester
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
        Card Gohda = new Card(3, 10, 4, CardEffects.GohdaEffect, "Gohda", new List<TagType> { TagType.Human, TagType.Servant }, true, false, 1);
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
        AddCardRegister(new Card(1, 2, 2, CardEffects.ShannonEffect, "Shannon", new List<TagType> { TagType.Human, TagType.Servant }, false, true, 2, requiresAI: true));
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

    public void Eva2Card()
    {
        Card Eva = new Card(1, 9, 3, CardEffects.EvaEffect, "Eva2", new List<TagType> { TagType.Human }, true, false, 0);
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

    public void Battler2Card()
    {
        Card Battler = new Card(1, 12, 1, CardEffects.BattlerEffect, "Battler2", new List<TagType> {TagType.Human }, false, false, 1);
        AddCardRegister(Battler);
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

    public void FutureGoatCard()
    {
        Card FutureGoat = new Card(1, 8, 8, CardEffects.FutureGoatEffect, "FutureGoat", new List<TagType> { TagType.Summon }, true, false, 0);
        AddCardRegister(FutureGoat);
    }

    public void GertrudeCard()
    {
        Card Gertrude = new Card(2, 20, 6, CardEffects.GertrudeEffect, "Gertrude", new List<TagType> { TagType.Human }, true, false, 0);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(Gertrude);
        }
        AddCardRegister(Gertrude);
    }

    public void CorneliaCard()
    {
        Card Cornelia = new Card(2, 20, 6, CardEffects.CorneliaEffect, "Cornelia", new List<TagType> { TagType.Human }, false, false, 2);
        AddCardRegister(Cornelia);
    }

    public void DlanorCard()
    {
        Card Dlanor = new Card(3, 8, 6, CardEffects.DlanorEffect, "Dlanor", new List<TagType> { TagType.Angel }, true, false, 0);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.EffectStopperList.Add(Dlanor);
        }
        AddCardRegister(Dlanor);
    }

    public void Chiester00Card()
    {
        Card Chiester00 = new Card(3, 30, 15, CardEffects.Chiester00Effect, "Chiester00", new List<TagType> { TagType.Chiester, TagType.Summon }, false, false, 2);
        AddCardRegister(Chiester00);
    }
    public void Chiester410Card()
    {
        Card Chiester410 = new Card(1, 8, 8, CardEffects.Chiester410Effect, "Chiester410", new List<TagType> { TagType.Chiester, TagType.Summon }, false, false, 1);
        AddCardRegister(Chiester410);
    }
    public void Chiester45Card()
    {
        Card Chiester45 = new Card(1, 8, 8, CardEffects.Chiester45Effect, "Chiester45", new List<TagType> { TagType.Chiester, TagType.Summon }, false, false, 1);
        AddCardRegister(Chiester45);
    }
    public void Chiester556Card()
    {
        Card Chiester556 = new Card(2, 20, 10, CardEffects.Chiester556Effect, "Chiester556", new List<TagType> { TagType.Chiester, TagType.Summon }, true, false, 1);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.TurnEndingList.Add(Chiester556);
        }
        AddCardRegister(Chiester556);
    }

    public void BernkastelCard()
    {
        Card Bernkastel = new Card(3, 36, 10, CardEffects.BernkastelEffect, "Bernkastel", new List<TagType> { TagType.Witch}, false, false, 1);
        AddCardRegister(Bernkastel);
    }

    public void AngeBeatriceCard()
    {
        Card AngeBeatrice = new Card(2, 36, 10, CardEffects.AngeBeatriceEffect, "AngeBeatrice", new List<TagType> { TagType.Witch }, false, false, 2);
        AddCardRegister(AngeBeatrice);
    }

    public void EvaBeatrice2Card()
    {
        Card EvaBeatrice = new Card(2, 36, 10, CardEffects.EvaBeatriceEffect, "EvaBeatrice2", new List<TagType> { TagType.Witch }, false, false, 1);
        AddCardRegister(EvaBeatrice);
    }

    public void PieceCard()
    {
        Card Piece = new Card(3, 36, 10, CardEffects.PieceEffect, "Piece", new List<TagType> { TagType.Witch }, false, false, 2);
        AddCardRegister(Piece);
    }

    public void CatsCard()
    {
        Card Cats = new Card(1, 1, 1, CardEffects.CatsEffect, "Cats", new List<TagType> { TagType.Cat }, true, false, 1);
        AddCardRegister(Cats);
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

    public static void RewardAllCards()
    {
        foreach(CardsByID cbi in Enum.GetValues(typeof(CardsByID)))
        {
            if (!cbi.ToString().StartsWith("PlaceHolder"))
            {
                AddCardToInventory(cbi);
            }
        }
    }

    private static void AddCardToInventory(CardsByID card, int number=1)
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
        AddCardToDeck(CardsByID.Eva2, saveCard: false);
        AddCardToDeck(CardsByID.George, saveCard: false);
        AddCardToDeck(CardsByID.Hideyoshi, saveCard: false);
        AddCardToDeck(CardsByID.Jessica, saveCard: false);
        AddCardToDeck(CardsByID.Krauss, saveCard: false);
        AddCardToDeck(CardsByID.Natsuhi, saveCard: false);
        AddCardToDeck(CardsByID.Rosa, saveCard: false);
    }

    public void EVA()
    {
        this.leaderCard = new Card(0, 100, 8, CardEffects.EvaBeatriceEffect, "EVA", new List<TagType> { TagType.Leader, TagType.Witch }, true, false, 1);
        AddCardRegister(this.leaderCard, true);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.MovementStopperList.Add(this.leaderCard);
        }
        AddCardToDeck(CardsByID.FutureGoat, 5, false);
        AddCardToDeck(CardsByID.Chiester00, saveCard: false);
        AddCardToDeck(CardsByID.Chiester410, saveCard: false);
        AddCardToDeck(CardsByID.Chiester45, saveCard: false);
        AddCardToDeck(CardsByID.Chiester556, saveCard: false);
    }

    public void Erika()
    {
        this.leaderCard = new Card(0, 100, 8, CardEffects.ErikaEffect, "Erika", new List<TagType> { TagType.Leader, TagType.Witch }, true, false, 1);
        AddCardRegister(this.leaderCard, true);
        if (EffectListener.INSTANCE != null)
        {
            EffectListener.INSTANCE.EffectStopperList.Add(this.leaderCard);
        }
        AddCardToDeck(CardsByID.FutureGoat, 20, false);
        AddCardToDeck(CardsByID.Cornelia, saveCard: false);
        AddCardToDeck(CardsByID.Gertrude, saveCard: false);
        AddCardToDeck(CardsByID.Dlanor, saveCard: false);
    }

    public void Featherine()
    {
        this.leaderCard = new Card(0, 160, 20, CardEffects.Featherineffect, "Featherine", new List<TagType> { TagType.Leader, TagType.Witch }, false, false, 1);
        AddCardRegister(this.leaderCard, true);
        AddCardToDeck(CardsByID.FutureGoat, 10, false);
        AddCardToDeck(CardsByID.Bernkastel, saveCard: false);
        AddCardToDeck(CardsByID.Piece, saveCard: false);
        AddCardToDeck(CardsByID.AngeBeatrice, saveCard: false);
        AddCardToDeck(CardsByID.EvaBeatrice2, saveCard: false);
    }

    // 1-6, 2-6, 3-6
    public static void LambdaReward()
    {
        int rn = new Random().Next(1, 31);
        CardsByID reward;
        List<CardsByID> possibleRewards;
        if (rn <= 5)
        {
            possibleRewards = new List<CardsByID> {CardsByID.Will};
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else if(rn <= 15)
        {
            possibleRewards = new List<CardsByID> {CardsByID.Lion};
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else
        {
            possibleRewards = new List<CardsByID> { CardsByID.Diana, CardsByID.Konpeitou };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        AddCardToInventory(reward);
    }

    public static void KinzoReward()
    {

        int rn = new Random().Next(1, 31);
        CardsByID reward;
        List<CardsByID> possibleRewards;
        if (rn <= 5)
        {
            possibleRewards = new List<CardsByID> { CardsByID.Gohda };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else if (rn <= 15)
        {
            possibleRewards = new List<CardsByID> { CardsByID.Genji, CardsByID.Nanjo};
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else
        {
            possibleRewards = new List<CardsByID> { CardsByID.Kanon, CardsByID.Shannon, CardsByID.Kumasawa};
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        AddCardToInventory(reward);
    }

    public static void AngeReward()
    {
        int rn = new Random().Next(1, 31);
        CardsByID reward;
        List<CardsByID> possibleRewards;
        if (rn <= 5)
        {
            possibleRewards = new List<CardsByID> { CardsByID.MariaBeatrice };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else if (rn <= 15)
        {
            possibleRewards = new List<CardsByID> { CardsByID.Maria, CardsByID.Lucifer };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else
        {
            possibleRewards = new List<CardsByID> { CardsByID.Asmodeus, CardsByID.Beelzebub, CardsByID.Belphegor, CardsByID.Leviathan, CardsByID.Mammon, CardsByID.Satan, CardsByID.Sakutarou };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        AddCardToInventory(reward);
    }

    public static void BattlerReward()
    {
        int rn = new Random().Next(1, 31);
        CardsByID reward;
        List<CardsByID> possibleRewards;
        if (rn <= 5)
        {
            possibleRewards = new List<CardsByID> { CardsByID.Natsuhi };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else if (rn <= 15)
        {
            possibleRewards = new List<CardsByID> {CardsByID.Rosa, CardsByID.Battler2};
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else
        {
            possibleRewards = new List<CardsByID> { CardsByID.Rudolf, CardsByID.Kyrie, CardsByID.Eva2, CardsByID.George, CardsByID.Jessica, CardsByID.Hideyoshi, CardsByID.Krauss };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        AddCardToInventory(reward);
    }

    public static void EVAReward()
    {
        int rn = new Random().Next(1, 31);
        CardsByID reward;
        List<CardsByID> possibleRewards;
        if (rn <= 5)
        {
            possibleRewards = new List<CardsByID> { CardsByID.Chiester00 };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else
        {
            possibleRewards = new List<CardsByID> { CardsByID.Chiester410, CardsByID.Chiester45, CardsByID.Chiester556, CardsByID.FutureGoat};
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        AddCardToInventory(reward);
    }

    public static void ErikaReward()
    {
        int rn = new Random().Next(1, 31);
        CardsByID reward;
        List<CardsByID> possibleRewards;
        if (rn <= 5)
        {
            possibleRewards = new List<CardsByID> { CardsByID.Dlanor };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else if (rn <= 15)
        {
            possibleRewards = new List<CardsByID> { CardsByID.Gertrude, CardsByID.Cornelia };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else
        {
            possibleRewards = new List<CardsByID> { CardsByID.FutureGoat };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        AddCardToInventory(reward);
    }

    public static void FeatherineReward()
    {
        int rn = new Random().Next(1, 31);
        CardsByID reward;
        List<CardsByID> possibleRewards;
        if (rn <= 5)
        {
            possibleRewards = new List<CardsByID> { CardsByID.Bernkastel, CardsByID.Piece };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else if (rn <= 15)
        {
            possibleRewards = new List<CardsByID> { CardsByID.AngeBeatrice, CardsByID.EvaBeatrice2 };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else
        {
            possibleRewards = new List<CardsByID> { CardsByID.Cats, CardsByID.FutureGoat };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        AddCardToInventory(reward);
    }
    public static void GohdaReward()
    {
        int rn = new Random().Next(1, 31);
        CardsByID reward;
        List<CardsByID> possibleRewards;
        if (rn <= 5)
        {
            possibleRewards = new List<CardsByID> { CardsByID.Dlanor };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else if (rn <= 15)
        {
            possibleRewards = new List<CardsByID> { CardsByID.Gertrude, CardsByID.Cornelia };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        else
        {
            possibleRewards = new List<CardsByID> { CardsByID.FutureGoat };
            int rnpr = new Random().Next(0, possibleRewards.Count);
            reward = possibleRewards[rnpr];
        }
        AddCardToInventory(reward);
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
        Eva2=25,
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
        Cats=42,
        Piece=43,
        AngeBeatrice=44,
        EvaBeatrice2=45,
        FutureGoat = 46,
        Battler2 = 47,
        PlaceHolder48 = 48,
        PlaceHolder49 = 49,
        PlaceHolder50 = 50
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
        new Card(3, 10, 4, CardEffects.GohdaEffect, "Gohda", new List<TagType> { TagType.Human, TagType.Servant }, true, false, 1),
        new Card(1, 6, 6, CardEffects.KanonEffect, "Kanon", new List<TagType> { TagType.Human, TagType.Servant }, true, false, 0),
        new Card(1, 2, 2, CardEffects.ShannonEffect, "Shannon", new List<TagType> { TagType.Human, TagType.Servant }, false, true, 2, requiresAI: true),
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
        new Card(2, 20, 6, CardEffects.RosaEffect, "Rosa", new List<TagType> { TagType.Human }, true, false, 0),
        new Card(3, 8, 10, CardEffects.WillEffect, "Will", new List<TagType> { TagType.Human }, true, false, 0),
        new Card(2, 6, 2, CardEffects.LionEffect, "Lion", new List<TagType> { TagType.Human}, false, false, 1, requiresAI: true),
        new Card(1, 2, 2, CardEffects.DianaEffect, "Diana", new List<TagType> { TagType.Cat, TagType.Pet }, true, false, 1),
        new Card(1, 1, 3, CardEffects.KonpeitouEffect, "Konpeitou", new List<TagType> { TagType.Summon, TagType.Object }, true, false, 0),
        new Card(3, 8, 6, CardEffects.DlanorEffect, "Dlanor", new List<TagType> { TagType.Angel }, true, false, 0),
        new Card(2, 20, 6, CardEffects.CorneliaEffect, "Cornelia", new List<TagType> { TagType.Human }, false, false, 2),
        new Card(2, 20, 6, CardEffects.GertrudeEffect, "Gertrude", new List<TagType> { TagType.Human }, true, false, 0),
        new Card(1, 8, 8, CardEffects.FutureGoatEffect, "FutureGoat", new List<TagType> { TagType.Summon }, true, false, 0),
        new Card(3, 30, 15, CardEffects.Chiester00Effect, "Chiester00", new List<TagType> { TagType.Chiester, TagType.Summon }, false, false, 2),
        new Card(1, 8, 8, CardEffects.Chiester410Effect, "Chiester410", new List<TagType> { TagType.Chiester, TagType.Summon }, false, false, 1),
        new Card(1, 8, 8, CardEffects.Chiester45Effect, "Chiester45", new List<TagType> { TagType.Chiester, TagType.Summon }, false, false, 1),
        new Card(2, 20, 10, CardEffects.Chiester556Effect, "Chiester556", new List<TagType> { TagType.Chiester, TagType.Summon }, true, false, 1),
        new Card(1, 12, 1, CardEffects.BattlerEffect, "Battler2", new List<TagType> {TagType.Human }, false, false, 1),
        new Card(3, 36, 10, CardEffects.BernkastelEffect, "Bernkastel", new List<TagType> { TagType.Witch }, false, false, 1),
        new Card(2, 36, 10, CardEffects.AngeBeatriceEffect, "AngeBeatrice", new List<TagType> { TagType.Witch }, false, false, 2),
        new Card(2, 36, 10, CardEffects.EvaBeatrice2Effect, "EvaBeatrice2", new List<TagType> { TagType.Witch }, false, false, 1),
        new Card(3, 36, 10, CardEffects.PieceEffect, "Piece", new List<TagType> { TagType.Witch }, false, false, 2),
        new Card(1, 1, 1, CardEffects.CatsEffect, "Cats", new List<TagType> { TagType.Cat }, true, false, 1),
    };
        return ret;
    }
}
