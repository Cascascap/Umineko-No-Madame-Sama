using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public int numberOfCards { get; set; }
    public List<Card> cards { get; set; }

    void Start()
    {
        cards = new List<Card>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
