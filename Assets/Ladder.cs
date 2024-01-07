using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }
    private int LevelMove;


    public GameObject Trigger(string floorID, int ObjectID)
    {
        ID = floorID+gameObject.name+ObjectID;


        LevelMove = PlayerPrefs.GetInt("CurrentLevel") + 1;

        return gameObject;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("You finna move");
        if (other.CompareTag("Player")) 
        {
            LevelPassedEvent levelPassed = new LevelPassedEvent()
            {
                MoveToLevel = LevelMove,
            };
            EventManager.Broadcast(levelPassed);
        }

    }
}
