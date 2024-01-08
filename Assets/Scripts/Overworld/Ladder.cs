using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// For moving to the next level
public class Ladder : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }
    private int LevelMove;


    public GameObject Trigger(string floorID, int ObjectID)
    {
        ID = floorID+"Object" +ObjectID;


        LevelMove = PlayerPrefs.GetInt("CurrentLevel") + 1;

        return gameObject;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("You finna move");
        if (other.CompareTag("PlayerRoaming")) 
        {
            LevelPassedEvent levelPassed = new LevelPassedEvent()
            {
                MoveToLevel = LevelMove,
            };
            EventManager.Broadcast(levelPassed);
        }

    }
}
