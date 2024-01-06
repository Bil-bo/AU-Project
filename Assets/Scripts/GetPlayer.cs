using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayer : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }



    public GameObject Trigger(FloorManager floor)
    {
        Destroy(gameObject);
        return null;
    }

    private void OnDestroy()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = transform.position;
    }

}
