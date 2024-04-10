using FishNet;
using FishNet.Connection;
using FishNet.Managing.Object;
using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : NetworkBehaviour
{

    public static GameManager gameManager;
    public static TextMeshProUGUI lifeText;
    public static RectTransform userboard;
    public static TextMeshProUGUI playerNames;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        lifeText = transform.Find("Life").GetComponent<TextMeshProUGUI>();
        userboard = transform.Find("Userboard").GetComponent<RectTransform>();
        playerNames = userboard.Find("PlayerNames").GetComponent<TextMeshProUGUI>();
    }

    [ObserversRpc]
    public void UpdateUserboard()
    {
        playerNames.text = "<b>Players:</b>";
        foreach (GameObject player in gameManager.players)
        {
            playerNames.text += "<br>"+player.name;
        }
    }

}
