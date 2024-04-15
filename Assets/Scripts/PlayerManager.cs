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
        AddPlayer();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("--- alguem saiu, e foi:" + Owner);
        RemovePlayer();
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

    [Server]
    void AddPlayer()
    {
        GameManager.instance.AddPlayerName(base.Owner);
        UpdateUserboard();
    }

    [Server]
    void RemovePlayer()
    {
        GameManager.instance.RemovePlayerName(base.Owner);
        UpdateUserboard();
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
