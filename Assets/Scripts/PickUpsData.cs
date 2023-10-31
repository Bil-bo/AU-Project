using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpsData : MonoBehaviour
{ 
    public GameObject door;

    public void PickedUp()
    {
        if (door != null)
        {
            GameData.Instance.setDoor(door.GetComponent<Door>(), true);
            door.SetActive(false);
        }
        this.gameObject.SetActive(false);
        
    }




}
