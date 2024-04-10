using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    public static GameManager instance;
    public readonly SyncList<string> playerNames = new();

    private void Awake()
    {
        instance = this;
    }

    List<string> names = new List<string>(){
        "Sombra", "Blitzkrieg", "Waifu", "Ghost", "VíboraCobra",
        "MagoDeEspada", "Tempestade", "RaioTrovão", "Zeus","PunhoDeFerro"
    };

    public void AddPlayerName(GameObject player)
    {
        string name = GenerateName();
        player.name = name;
        playerNames.Add(name);
    }

    string GenerateName()
    {
        string name;
        do
        {
            name = names[Random.Range(0, names.Count)];
        } 
        while (playerNames.Contains(name));

        return name;
    }

}
