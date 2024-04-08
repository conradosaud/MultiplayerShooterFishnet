using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthPoints : NetworkBehaviour
{

    public float healthPoints = 5;

    private void Awake()
    {
        IncreaseHP(healthPoints);
    }

    [ServerRpc]
    public void DecreaseHP(float value = 1)
    {
        healthPoints-= value;
        SetLifeText();
    }

    [ServerRpc]
    public void IncreaseHP(float value = 1)
    {
        healthPoints+= value;
        SetLifeText();
    }

    [ObserversRpc]
    void SetLifeText()
    {
        HUDController.lifeText.text = healthPoints.ToString();
    }



}
