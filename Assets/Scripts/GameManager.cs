using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    public static GameManager instance;
    public readonly SyncList<GameObject> players = new();

    private void Awake()
    {
        instance = this;
    }

    List<string> names = new List<string>(){
        "Sombra", "Blitzkrieg", "Waifu", "Ghost", "VíboraCobra",
        "MagoDeEspada", "Tempestade", "RaioTrovão", "Zeus","PunhoDeFerro"
    };

    public string AddPlayer(GameObject player)
    {
        string name = GenerateName();
        player.name = name;
        instance.players.Add(player);
        return name;
    }

    string GenerateName()
    {
        string name = names[Random.Range(0, names.Count)];
        foreach (GameObject player in players)
        {
            if (player.name == name)
                GenerateName();
        }
        return name;
    }

}
