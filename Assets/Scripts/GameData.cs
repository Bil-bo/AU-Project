using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    private Dictionary<string, bool> doors = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddDoor(Door door)
    {
        doors.Add(door.ID, door.isOpen);    
    }

    public void LoadDoor(Door door)
    {
        if (doors.ContainsKey(door.ID))
        {
            door.isOpen = doors[door.ID];
        }
    }

    public void setDoor(Door door, bool newState)
    {
        if (doors.ContainsKey(door.ID)) 
        {
            doors[door.ID] = newState;
        }
    }

    public bool HasDoors()
    {
        return doors.Count > 0;
    }
}