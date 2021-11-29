using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public Card(int cost, int hp, int attack, Action effect, string imageName, List<string> tags, int cooldown)
    {
        this.Cost = cost;
        this.HP = hp;
        this.Attack = attack;
        this.Effect = effect;
        this.ImageName = imageName;
        this.tags = tags;
        this.Cooldown = cooldown;
    }

    public int Cost { get; set; }
    public int HP { get; set; }
    public int Attack { get; set; }
    public int Counters { get; set; }
    public Action Effect { get; set; }
    public string ImageName { get; set; }
    public int Cooldown { get; set; }
    public List<string> tags { get; set; }


}
