using FishNet;
using FishNet.Connection;
using FishNet.Managing.Object;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{

    public static HUDController instance;

    public static TextMeshProUGUI lifeText;
    public static RectTransform userboard;
    public static TextMeshProUGUI playerNames;

    private void Awake()
    {
        instance = this;

        lifeText = transform.Find("Life").GetComponent<TextMeshProUGUI>();
        userboard = transform.Find("Userboard").GetComponent<RectTransform>();
        playerNames = userboard.Find("PlayerNames").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateUserboard(string[] names)
    {
        playerNames.text = "<b>Players:</b>";
        for (int i = 0; i < names.Length; i++)
        {
            playerNames.text += "<br>" + names[i];
        }
    }

}
