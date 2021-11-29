using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Deck 
{
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
        GameStart.INSTANCE.CardsInGame.Add(card);
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
        this.leaderCard = new Card(0, 36, 3, CardEffects.BeatriceEffect, "Beatrice", new List<string> { "Leader, Witch" }, 1);
        AddCardRegister(this.leaderCard, true);
        Card goat = new Card(1, 2, 2, CardEffects.GoatEffect, "Goat", new List<string> { "Summoned" }, 0);
        AddCardRegister(goat, times:10);
        AddCardRegister(new Card(1, 4, 4, CardEffects.AsmodeusEffect, "Asmodeus", new List<string> {"Summoned", "Stake"},1 ));
        AddCardRegister(new Card(1, 5, 3, CardEffects.BeelzebubEffect, "Beelzebub", new List<string> { "Summoned", "Stake" }, 0));
        AddCardRegister(new Card(1, 6, 2, CardEffects.BelphegorEffect, "Belphegor", new List<string> { "Summoned", "Stake" }, 0));
        AddCardRegister(new Card(1, 4, 4, CardEffects.LeviathanEffect, "Leviathan", new List<string> { "Summoned", "Stake" }, 1));
        AddCardRegister(new Card(1, 4, 4, CardEffects.MammonEffect, "Mammon", new List<string> { "Summoned", "Stake" }, 2));
        AddCardRegister(new Card(1, 2, 6, CardEffects.SatanEffect, "Satan", new List<string> { "Summoned", "Stake" }, 0));
        AddCardRegister(new Card(2, 7, 7, CardEffects.LuciferEffect, "Lucifer", new List<string> { "Summoned", "Stake" }, 0));
        AddCardRegister(new Card(2, 4, 4, CardEffects.GaapEffect, "Gaap", new List<string> { "Summoned", "Demon" }, 0));
        AddCardRegister(new Card(2, 8, 4, CardEffects.RonoveEffect, "Ronove", new List<string> { "Summoned", "Demon" }, 1));
        AddCardRegister(new Card(3, 12, 6, CardEffects.VirgiliaEffect, "Virgilia", new List<string> { "Witch" }, 1));
    }

    public void Lambda()
    {
        this.leaderCard = new Card(0, 36, 3, CardEffects.LambdaEffect, "Lambda", new List<string> { "Leader, Witch" }, 1);
        AddCardRegister(this.leaderCard, true);
        Card Konpeitou = new Card(1, 1, 3, CardEffects.KonpeitouEffect, "Konpeitou", new List<string> { "Summoned", "Object" }, 0);
        AddCardRegister(Konpeitou, times:4);
        AddCardRegister(new Card(3, 8, 10, CardEffects.WillEffect, "Will", new List<string> { "Human" }, 0));
        AddCardRegister(new Card(1, 6, 2, CardEffects.LionEffect, "Lion", new List<string> { "Human" }, 1));
        AddCardRegister(new Card(1, 2, 2, CardEffects.DianaEffect, "Diana", new List<string> { "Cat", "Pet" }, 1));
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
