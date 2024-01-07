using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayer : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }
    
    public GameObject Trigger(string floorID, int ObjectID)
    {

        ID = floorID+gameObject.name+ObjectID;

        if (!PlayerPrefs.HasKey(ID))
        {
            PlayerPrefs.SetInt(ID, 0);

            return gameObject;
        }

        else
        {
            PlayerPrefs.SetInt(ID, 1);
            Debug.Log("Should pull now");
            StartCoroutine(PullPlayer());

            return null;
        }


    }

    private IEnumerator PullPlayer()
    {
        yield return null;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(transform.position);
        player.transform.position = transform.position;
        

    }

}
