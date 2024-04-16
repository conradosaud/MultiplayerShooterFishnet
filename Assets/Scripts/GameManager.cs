using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    public static GameManager instance;
    public readonly SyncDictionary<NetworkConnection, string> playerNames = new(new SyncTypeSettings( WritePermission.ClientUnsynchronized) );

    List<string> names = new List<string>(){
        "Sombra", "Blitzkrieg", "Waifu", "Ghost", "V�boraCobra",
        "MagoDeEspada", "Tempestade", "RaioTrov�o", "Zeus","PunhoR�pido"
    };

    private void Awake()
    {
        instance = this;
    }

    public void AddPlayerName(NetworkConnection conn)
    {
        playerNames.Add(conn, GenerateName());
    }

    public void RemovePlayerName(NetworkConnection conn)
    {
        playerNames.Remove(conn);
    }

    string GenerateName()
    {

        string name = names[Random.Range(0, names.Count)];
        bool found = false;

        foreach ( var item in playerNames )
        {
            if( item.Value == name) 
            {
                found = true;
                break;
            }
        }

        if (found == false)
            return name;

        return GenerateName();
        
    }

}
