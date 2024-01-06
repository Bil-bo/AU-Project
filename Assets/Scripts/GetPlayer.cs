using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayer : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }



    public GameObject Trigger(string floorID, int ObjectID)
    {
        Destroy(gameObject);
        return null;
    }

    private void OnDestroy()
    {
        try
        { 
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = transform.position; 
        }
        catch { }
    }

}
