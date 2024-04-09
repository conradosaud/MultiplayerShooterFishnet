using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{

    public static TextMeshProUGUI lifeText;

    // Start is called before the first frame update
    void Start()
    {
        lifeText = GameObject.Find("Canvas").transform.Find("HUD").transform.Find("Life").GetComponent<TextMeshProUGUI>();
    }
}
