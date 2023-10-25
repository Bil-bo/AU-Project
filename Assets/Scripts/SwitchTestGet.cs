using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwitchTestGet : MonoBehaviour
{

    public TextMeshProUGUI testing;
    // Start is called before the first frame update
    void Start()
    {
        testing.text = "Value: " + GameData.EnemyValue;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
