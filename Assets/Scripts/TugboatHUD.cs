using FishNet.Transporting.Tugboat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TugboatHUD : MonoBehaviour
{

    TMP_InputField input;
    Tugboat tugboat;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<TMP_InputField>();
        tugboat = GameObject.Find("NetworkManager").GetComponent<Tugboat>();

        input.onValueChanged.AddListener( (texto) => tugboat.SetClientAddress(texto) );

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
