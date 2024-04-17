using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : NetworkBehaviour
{

    public GameObject playerPrefab;

    [ServerRpc(RequireOwnership = false)]
    public void Respawn()
    {
        GameObject.Find("Canvas").transform.Find("HUD").transform.Find("Dead").GetComponent<Canvas>().enabled = false;
        GameObject player = Instantiate(playerPrefab);
        ServerManager.Spawn(player, base.Owner);
    }
}
