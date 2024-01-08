using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// On level init pull player to start point
public class GetPlayer : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }
    
    public GameObject Trigger(string floorID, int ObjectID)
    {

        ID = floorID+"Object" +ObjectID;

        if (!PlayerPrefs.HasKey(ID))
        {
            PlayerPrefs.SetInt(ID, 0);

            return gameObject;
        }

        else
        {
            if (PlayerPrefs.GetInt(ID, 0) == 0)
            {

                PlayerPrefs.SetInt(ID, 1);


                Debug.Log("Should pull now");
                StartCoroutine(PullPlayer());
            }
            return null;
        }


    }

    // In case scene is loading too fast
    private IEnumerator PullPlayer()
    {
        yield return new WaitForSeconds(1f);
        GameObject player = GameObject.FindGameObjectWithTag("PlayerRoaming");
        Debug.Log(transform.position);
        player.transform.position = transform.position;
        PlayerPrefs.SetFloat("PlayerX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", transform.position.z);


    }

}
