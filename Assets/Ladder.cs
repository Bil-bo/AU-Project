using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IFloorObject, IOnBossDefeated
{
    public string ID { get; set; }
    private int LevelMove;


    public GameObject Trigger(string floorID, int ObjectID)
    {
        EventManager.AddListener<BossDefeatedEvent>(OnBossDefeated);
        ID = floorID+gameObject.name+ObjectID;
        Debug.Log(LevelMove);


        LevelMove = PlayerPrefs.GetInt("CurrentLevel") + 1;

        return gameObject;
    }

    public void OnBossDefeated(BossDefeatedEvent eventData)
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            LevelPassedEvent levelPassed = new LevelPassedEvent()
            {
                MoveToLevel = LevelMove,
            };

            EventManager.Broadcast(levelPassed);
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<BossDefeatedEvent>(OnBossDefeated);
    }


}
