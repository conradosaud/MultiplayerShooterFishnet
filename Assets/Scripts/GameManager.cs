using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public readonly SyncList<GameObject> players = new();

    List<string> names = new List<string>(){
        "SombraMatadora", "Blitzkrieg", "Ceifador", "Ghost", "VíboraCobra",
        "Guerra", "Tempestade", "RaioTrovão", "Trovador","PunhoDeFerro"
    };

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayer(GameObject player)
    {
        player.name = GenerateName();
        players.Add(player);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemovePlayer(GameObject player)
    {
        players.Remove(player);
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
