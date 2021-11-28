using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFunctions : MonoBehaviour
{
    public static AIFunctions INSTANCE = null;
    public Hand hand;

    // Start is called before the first frame update
    void Start()
    {
        INSTANCE = this;
    }


    public void TakeTurn(Deck enemyDeck)
    {
        GameStart.INSTANCE.OnTurnStart();
    }
}
