using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameText : NetworkBehaviour
{

    public override void OnStartClient()
    {
        base.OnStartClient();

        if( IsOwner)
        {
            transform.GetComponent<TextMeshProUGUI>().enabled = false;
        }

    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
