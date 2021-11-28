using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFunctions : MonoBehaviour
{
    public static AIFunctions INSTANCE = null;
    public Deck deck;
    public Hand hand;

    // Start is called before the first frame update
    void Start()
    {
        INSTANCE = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeTurn()
    {

    }
}
