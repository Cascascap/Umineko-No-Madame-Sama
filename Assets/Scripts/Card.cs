using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{

    public Card(int cost, int hp, int attack, Action effect, string imageName, List<string> tags)
    {
        this.cost = cost;
        this.hp = hp;
        this.attack = attack;
        this.effect = effect;
        this.imageName = imageName;
        this.tags = tags;
    }

    public int cost { get; set; }
    public int hp { get; set; }
    public int attack { get; set; }
    public int positionX { get; set; }
    public int positionY { get; set; }
    public bool acted { get; set; }
    public bool usedEffect { get; set; }
    public Action effect { get; set; }
    public int counters { get; set; }
    public string imageName { get; set; }
    public List<string> tags { get; set; }

    internal static Dictionary<string, Card> getAllCards()
    {
        Dictionary<string, Card> allCards = new Dictionary<string, Card>();

        //allCards.Add("Beatrice", new Card(0, 36, 3, CardEffects.BeatriceEffect, "Beatrice"));

        return allCards;
    }
}
