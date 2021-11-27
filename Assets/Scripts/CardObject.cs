using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject
{
    public CardObject(GameObject gameObject)
    {
        GameObject = gameObject;
    }

    public GameObject cardObject { get; set; }
    public bool acted { get; set; }
    public bool usedEffect { get; set; }
    public bool moved { get; set; }
    public GameObject GameObject { get; }
}
