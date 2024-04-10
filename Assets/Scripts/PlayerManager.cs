using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
            AddPlayer();
    }

    [ServerRpc]
    void AddPlayer()
    {
        string name = GameManager.instance.AddPlayer(gameObject);
        transform.GetComponent<PlayerController>().username = name;
        UpdateUserboard();
    }

    [ObserversRpc]
    void UpdateUserboard()
    {
        HUDController.instance.UpdateUserboard();
    }

}
