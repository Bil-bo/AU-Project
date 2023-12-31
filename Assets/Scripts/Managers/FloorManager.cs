using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Manager for every floor object
public class FloorManager : MonoBehaviour
{
    public List<Door> DoorList =new();

    private bool _Clear = false;

    public bool Clear 
    {
        get { return _Clear; }
        set 
        {
            _Clear = value; 
            if (value) 
            { 
                PlayerPrefs.SetInt(FloorID, 1);
                DoorList.ForEach(door => door.SetState(false));
            }
        }
        

    }
    public string FloorID { private get; set; }

    private bool InRoom = false;

    public List<GameObject> Uncleared = new();


    // Called in MainGameManager, to set up each child in the floor
    public void Initialise(Vector2Int coordinate, int level)
    {
        FloorID = "Level"+level+coordinate+gameObject.name;

        if (PlayerPrefs.HasKey(FloorID))
        {
            Clear = PlayerPrefs.GetInt(FloorID) == 1;
        }

        else
        {
            PlayerPrefs.SetInt(FloorID, 0);
        }

        int i = 0;
        foreach (IFloorObject f in transform.GetComponentsInChildren<IFloorObject>(true))
        {
            GameObject floorObject = f.Trigger(FloorID, i);
            if (floorObject != null) { Uncleared.Add(floorObject); i++; }
                
        }

        if (Uncleared.Count == 0)
        {
            Clear = true;
            PlayerPrefs.SetInt(FloorID, 1);
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("PlayerRoaming"))
        {
            // Start a timer
            InRoom = true; 
            if (!Clear) { StartCoroutine(CountDown()); }

            else
            {
                // If the room is clear, set save point
                PlayerPrefs.SetFloat("PlayerX", transform.position.x);
                PlayerPrefs.SetFloat("PlayerY", other.transform.position.y);
                PlayerPrefs.SetFloat("PlayerZ", transform.position.z);
            }
        }
    }

    public void AddToList(GameObject obj)
    {
        Uncleared.Add(obj);
    }

    public void RemoveFromList(GameObject obj)
    {
        Uncleared.Remove(obj);
        if (Uncleared.Count >= 0) { }
        Clear = Uncleared.Count <= 0;
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop the timer
        if (other.CompareTag("PlayerRoaming"))
        {
            InRoom = false;
        }

    }

    // Lock the doors
    private IEnumerator CountDown() 
    {
        yield return new WaitForSeconds(1f);

        if (InRoom) { DoorList.ForEach(door => door.SetState(true)); }


    }

}
