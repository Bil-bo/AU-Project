using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpsData : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }

    public void PickedUp()
    {
        this.gameObject.SetActive(false);
        
    }

    public GameObject Trigger(string floorID, int ObjectID)
    {
        ID = floorID+"Object" +ObjectID;

        if (PlayerPrefs.HasKey(ID)) 
        {
            if (PlayerPrefs.GetInt(ID) == 1) 
            {
                gameObject.SetActive(false);
            }
        }

        else
        {
            PlayerPrefs.SetInt(ID, 0);
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickupCollectedEvent gameEvent = new PickupCollectedEvent()
            {
                PickUp = this
            };

            EventManager.Broadcast(gameEvent);

            PlayerPrefs.SetInt(ID, 1);
            gameObject.SetActive(false);

        }
    }

}
