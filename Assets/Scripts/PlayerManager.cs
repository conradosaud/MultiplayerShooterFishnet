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
        GameManager.instance.AddPlayerName(Owner);
        UpdateUserboard();
        //UpdateHeadUsername();
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
            Debug.Log(headUsernameText);
            headUsernameText.text = GameManager.instance.playerNames[Owner];
        }
    }

}
