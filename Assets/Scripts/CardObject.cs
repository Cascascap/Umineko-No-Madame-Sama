using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject
{

    public CardObject(GameObject gameObject)
    {
        GameObject = gameObject;
        TurnEffectWasUsedOn = 0;
    }

    public Card card { get; set; }
    public bool acted { get; set; }
    public bool usedEffect { get; set; }
    public bool moved { get; set; }
    public int currentHP { get; set; }
    public int currentATK { get; set; }
    public int counters { get; set; }
    public int TurnEffectWasUsedOn { get; set; }
    public GameObject GameObject { get; set; }

    internal bool IsEnemyCard()
    {
        return GameObject.transform.parent.parent.name == GameStart.INSTANCE.EnemyField.name;
    }
}
