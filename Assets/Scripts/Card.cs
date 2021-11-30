using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Deck;

public class Card
{
    public Card(int cost, int hp, int attack, Action<GameObject> effect, string imageName, List<string> tags, bool automaticEffect, int cooldown, TargetType targetTpe = TargetType.Both)
    {
        this.Cost = cost;
        this.HP = hp;
        this.Attack = attack;
        this.Effect = effect;
        this.ImageName = imageName;
        this.tags = tags;
        this.AutomaticEffect = automaticEffect;
        this.TargetType = targetTpe;
        this.Cooldown = cooldown;
    }

    public int Cost { get; set; }
    public int HP { get; set; }
    public int Attack { get; set; }
    public int Counters { get; set; }
    public Action<GameObject> Effect { get; set; }
    public string ImageName { get; set; }
    public int Cooldown { get; set; }
    public bool AutomaticEffect { get; set; }
    public TargetType TargetType { get; set; }
    public List<string> tags { get; set; }


}
