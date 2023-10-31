using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class DoorInit : MonoBehaviour
{
    public List<GameObject> doors = new List<GameObject>();

    private void Start()
    {

        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].GetComponent<Door>().Initialise("Door" + i, false);
            if (GameData.Instance != null)
            {
                if (!GameData.Instance.FindDoor(doors[i].GetComponent<Door>()))
                {
                    GameData.Instance.AddDoor(doors[i].GetComponent<Door>());
                }
                else
                {
                    GameData.Instance.LoadDoor(doors[i].GetComponent<Door>());
                }
            }

            if (doors[i].GetComponent <Door>().isOpen)
            {
                Debug.Log("This is open");
                doors[i].SetActive(false);
            }

        }
    }
}
