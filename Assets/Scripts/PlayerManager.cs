using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{

    public override void OnStartServer()
    {
        base.OnStartServer();
        AddPlayer();
    }

    [Server]
    void AddPlayer()
    {
        GameManager.instance.AddPlayerName(gameObject);
        UpdateUserboard();
    }

    [ObserversRpc(BufferLast = true)]
    void UpdateUserboard()
    {
        HUDController.instance.UpdateUserboard();
    }

}
