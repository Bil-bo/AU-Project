using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RescueSpawn : MonoBehaviour, IFloorObject, IOnPlayerRescued
{
    public string ID { get; set; }
    private RescueInfo Rescue = new();


    private void Awake()
    {
        Debug.Log("morning");
        EventManager.AddListener<PlayerRescuedEvent>(OnPlayerRescued);
    }

    public GameObject Trigger(string floorID, int objectID)
    {
        
        ID = floorID + "Object" + objectID;

        GameObject player;

        if (PlayerPrefs.HasKey(ID))
        {

            Rescue = JsonUtility.FromJson<RescueInfo>(PlayerPrefs.GetString(ID));

            if (Rescue.Clear)
            {
                return null;
            }

            else
            {

                Addressables.LoadAssetAsync<GameObject>(Rescue.RescueID).Completed += (result) =>
                {
                    player = Instantiate(result.Result, transform.position, Quaternion.identity, transform.parent);
                    //player.transform.SetParent(transform.parent, true);
                    RescuePropsBound playerData = player.GetComponent<RescuePropsBound>();
                    playerData.SpawnerID = ID;
                };

                return gameObject;
            }
        }

        else
        {
            Debug.Log("Making New Player");
            Rescue.Clear = false;
            StartCoroutine(AddressablesManager.Instance.GetRandomItem(AddressType.SUPPORT, result =>
            {
                Rescue.RescueID = result.Key;
                player = Instantiate(result.Value, transform.position, Quaternion.identity);
                RescuePropsBound playerData = player.GetComponent<RescuePropsBound>();
                playerData.SpawnerID = ID;
                player.transform.SetParent(transform.parent, true);
                PlayerPrefs.SetString(ID, JsonUtility.ToJson(Rescue));

            }));

            return gameObject;
            
        }    
    }

    public void OnPlayerRescued(PlayerRescuedEvent eventData)
    {
        Debug.Log("Been Called");
        if (eventData.SpawnerID == ID)
        {
            Debug.Log("That's Me");
            transform.parent.GetComponent<FloorManager>().RemoveFromList(gameObject);
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<PlayerRescuedEvent>(OnPlayerRescued);
    }
}



public class RescueInfo : SaveInfo
{
    public string RescueID;
}
