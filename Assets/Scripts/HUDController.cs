using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{

    public static TextMeshProUGUI lifeText;

    // Start is called before the first frame update
    void Start()
    {
        lifeText = transform.Find("Life").GetComponent<TextMeshProUGUI>();
    }

}
