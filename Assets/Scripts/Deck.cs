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
    public Card playerLeaderCard;
    public Card enemyLeaderCard;

    public void InitializeDeck(string leaderName)
    {
        this.cards = CallMethod(leaderName);
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

    //Default decks:
    public Stack<Card> Beatrice()
    {
        this.playerLeaderCard = new Card(0, 36, 3, CardEffects.BeatriceEffect, "Beatrice", new List<string> { "Leader, Witch" });
        Stack<Card> deck = new Stack<Card>();
        for (int i = 0; i < 10; i++){
            deck.Push(new Card(1, 2, 2, CardEffects.GoatEffect, "Goat", new List<string>{"Summoned" }));
        }
        deck.Push(new Card(1, 4, 4, CardEffects.AsmodeusEffect, "Asmodeus", new List<string> {"Summoned", "Stake"}));
        deck.Push(new Card(1, 5, 3, CardEffects.BeelzebubEffect, "Beelzebub", new List<string> { "Summoned", "Stake" }));
        deck.Push(new Card(1, 6, 2, CardEffects.LionEffect, "Belphegor", new List<string> { "Summoned", "Stake" }));
        deck.Push(new Card(1, 4, 4, CardEffects.LeviathanEffect, "Leviathan", new List<string> { "Summoned", "Stake" }));
        deck.Push(new Card(1, 4, 4, CardEffects.MammonEffect, "Mammon", new List<string> { "Summoned", "Stake" }));
        deck.Push(new Card(1, 2, 6, CardEffects.SatanEffect, "Satan", new List<string> { "Summoned", "Stake" }));
        deck.Push(new Card(2, 7, 7, CardEffects.LuciferEffect, "Lucifer", new List<string> { "Summoned", "Stake" }));
        deck.Push(new Card(2, 4, 4, CardEffects.GaapEffect, "Gaap", new List<string> { "Summoned", "Demon" }));
        deck.Push(new Card(2, 8, 4, CardEffects.RonoveEffect, "Ronove", new List<string> { "Summoned", "Demon" }));
        deck.Push(new Card(3, 12, 6, CardEffects.VirgiliaEffect, "Virgilia", new List<string> { "Witch"}));
        return deck;
    }

    public Stack<Card> Lambda()
    {
        Stack<Card> deck = new Stack<Card>();
        for (int i = 0; i < 4; i++)
        {
            deck.Push(new Card(1, 1, 3, CardEffects.KonpeitouEffect, "Konpeitou", new List<string> { "Summoned", "Object" }));
        }
        deck.Push(new Card(3, 8, 10, CardEffects.WillEffect, "Will", new List<string> { "Human" }));
        deck.Push(new Card(1, 6, 2, CardEffects.LionEffect, "Lion", new List<string> { "Human" }));
        deck.Push(new Card(1, 2, 2, CardEffects.DianaEffect, "Diana", new List<string> { "Cat", "Pet" }));
        return deck;
    }

    public Card FindCardInDeck(string name)
    {
        List<Card> cardsList = this.cards.ToList(); 
        if(name == playerLeaderCard.imageName)
        {
            return playerLeaderCard;
        }
        if(name == enemyLeaderCard.imageName)
        {
            return enemyLeaderCard;
        }
        foreach (Card c in cardsList)
        {
            if(c.imageName == name)
            {
                return c;
            }
        }
        return null;
    }
}
