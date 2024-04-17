using FishNet;
using FishNet.Connection;
using FishNet.Managing.Client;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{

    //TextMeshProUGUI headUsernameText;

    public override void OnStartClient()
    {
        base.OnStartClient();
        //Server_UpdateUserboard();

        //base.ClientManager.OnClientConnectionState += (state) => Debug.Log("Se liga: --- "+state.ConnectionState);

        if (base.IsOwner)
        {
            //Debug.Log("é o odono");
            //AddPlayerName(base.Owner);
        }
        //UpdateHeadUsername();
    }

    //public override void OnStartServer()
    //{
    //    base.OnStartServer();
    //    GameManager.instance.AddPlayerName(base.Owner);
    //    UpdateUserboard();
    //}

    //public override void OnStopServer()
    //{
    //    base.OnStopServer();
    //    GameManager.instance.RemovePlayerName(base.Owner);
    //    //UpdateUserboard();
    //}

    //public override void OnStopNetwork()
    //{
    //    base.OnStopNetwork();
    //    GameManager.instance.RemovePlayerName(base.Owner);
    //    //UpdateUserboard();
    //}

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    UpdateHeadUsername();
    //}

    //private void Awake()
    //{
    //    headUsernameText = transform.Find("CanvasPlayerName").transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
    //}



    //[ObserversRpc(BufferLast = true)]
    //void UpdateHeadUsername()
    //{
    //    if (GameManager.instance.playerNames.ContainsKey(Owner))
    //    {
    //        headUsernameText.text = GameManager.instance.playerNames[Owner];
    //    }
    //}

}
