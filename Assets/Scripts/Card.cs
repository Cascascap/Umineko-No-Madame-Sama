using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Deck;

public class Card
{
    public Card(int cost, int hp, int attack, Func<Card, bool> effect, string imageName, List<TagType> tags, bool passiveEffect, bool usesTarget, int cooldown, TagType targetTag = TagType.All, TargetType targetTpe = TargetType.Both)
    {
        this.Cost = cost;
        this.HP = hp;
        this.Attack = attack;
        this.Effect = effect;
        this.ImageName = imageName;
        this.Tags = tags;
        this.PassiveEffect = passiveEffect;
        this.UsesTarget = usesTarget;
        this.TargetTag = targetTag;
        this.TargetType = targetTpe;
        this.Cooldown = cooldown;
    }


    public int Cost { get; set; }
    public int HP { get; set; }
    public int Attack { get; set; }
    public int Counters { get; set; }
    public Func<Card, bool> Effect { get; set; }
    private Dictionary<int, object> EffectParameters { get; set; }
    public string ImageName { get; set; }
    public TagType TargetTag { get; set; }
    public int Cooldown { get; set; }
    public bool PassiveEffect { get; set; }
    public bool UsesTarget { get; set; }
    public TargetType TargetType { get; set; }
    public List<TagType> Tags { get; set; }

    public void InitializeEffectParametrs()
    {
        this.EffectParameters = new Dictionary<int, object>();
    }
    public void SetTargetCard(GameObject target)
    {
        this.EffectParameters.Add(1, target);
    }

    public GameObject GetTargetCard()
    {
        return (GameObject) this.EffectParameters[1];
    }

    public void SetTargetCardTags(List<TagType> targetTags)
    {
        this.EffectParameters.Add(2, targetTags);
    }

    public List<TagType> GetTargetCardTags()
    {
        return (List<TagType>)this.EffectParameters[2];
    }

    public void SetCounters(int counters)
    {
        this.EffectParameters.Add(3, counters);
    }

    public int GetCounters()
    {
        return (int) this.EffectParameters[3];
    }
}
