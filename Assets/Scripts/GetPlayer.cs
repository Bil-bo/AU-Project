using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayer : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }
    
    public GameObject Trigger(string floorID, int ObjectID)
    {
        ID = floorID+gameObject.name+ObjectID;
        Debug.Log("Grabbing Player");
        if (!PlayerPrefs.HasKey(ID))
        {
            PlayerPrefs.SetString(ID, ID);
            Debug.Log("Initial Load");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = transform.position;
        }

        return null;
    }

}
