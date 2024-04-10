using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{

    public override void OnStartClient()
    {
        base.OnStartClient();


        if ( IsOwner )
            GameObject.Find("GameManager").GetComponent<GameManager>().AddPlayer(gameObject);

        
        //HUDController.UpdateUserboard();

        Debug.Log(GameObject.Find("GameManager").GetComponent<GameManager>().players.Count);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if( IsOwner )
            GameObject.Find("GameManager").GetComponent<GameManager>().RemovePlayer(gameObject);

        //HUDController.UpdateUserboard();
    }

}
