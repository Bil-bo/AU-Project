using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Destroy(gameObject);
        
    }

    private void OnDestroy()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = transform.position;

    }

}
