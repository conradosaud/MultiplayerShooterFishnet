using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{

    TextMeshProUGUI headUsernameText;

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameManager.instance.AddPlayerName(base.Owner);
        UpdateUserboard();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        GameManager.instance.RemovePlayerName(base.Owner);
        //UpdateUserboard();
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        GameManager.instance.RemovePlayerName(base.Owner);
        //UpdateUserboard();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        UpdateHeadUsername();
    }

    private void Awake()
    {
        headUsernameText = transform.Find("CanvasPlayerName").transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
    }

    [ObserversRpc(BufferLast = true)]
    void UpdateUserboard()
    {
        HUDController.instance.UpdateUserboard();
    }


    [ObserversRpc(BufferLast = true)]
    void UpdateHeadUsername()
    {
        if (GameManager.instance.playerNames.ContainsKey(Owner))
        {
            headUsernameText.text = GameManager.instance.playerNames[Owner];
        }
    }

}
