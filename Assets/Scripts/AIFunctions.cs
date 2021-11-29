using System;
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
        if (!IsLeaderOnTop())
        {
            MoveLeaderTop();
        }
        GameStart.INSTANCE.OnTurnStart();
    }

    private void MoveLeaderTop()
    {
        //throw new NotImplementedException();
    }

    public bool IsLeaderOnTop()
    {
        List<CardObject> allCards= GameStart.INSTANCE.CardGameObjectsInGame;
        CardObject enemyLeader = allCards.Find(x => x.GameObject.name == GameStart.INSTANCE.EnemyLeader + "Card");
        string slotName = enemyLeader.GameObject.transform.parent.name;
        int yCoordinates = int.Parse(slotName.Substring(8, 1));
        return yCoordinates == 1;
    }
}
