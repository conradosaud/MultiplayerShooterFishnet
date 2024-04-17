using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    public static GameManager instance;
    public readonly SyncDictionary<NetworkConnection, string> playerNames = new();

    List<string> names = new List<string>(){
        "Sombra", "Blitzkrieg", "Waifu", "Ghost", "VíboraCobra",
        "MagoDeEspada", "Tempestade", "RaioTrovão", "Zeus","PunhoRápido"
    };

    private void Awake()
    {
        instance = this;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        playerNames.Clear(); // limpar lixo de conexões anteriores
    }

    public override void OnSpawnServer(NetworkConnection connection)
    {
        base.OnSpawnServer(connection);

        playerNames.Add(connection, GenerateName());
        UpdateUserboard(playerNames.Values.ToArray());
    }

    public override void OnDespawnServer(NetworkConnection connection)
    {
        base.OnSpawnServer(connection);

        playerNames.Remove(connection);
        UpdateUserboard(playerNames.Values.ToArray());
    }

    [ObserversRpc(BufferLast = true)]
    void UpdateUserboard(string[] names)
    {
        HUDController.instance.UpdateUserboard( names );
    }

    public string GetMyName(NetworkConnection conn)
    {
        foreach( var item in playerNames)
        {
            if( item.Key == conn )
                return item.Value;
        }
        return "Unkwon";
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
