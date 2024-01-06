using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

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
            if (value) { PlayerPrefs.SetInt(FloorID, 1); }
        }
        

    }
    public string FloorID { private get; set; }

    private bool InRoom = false;

    private List<GameObject> _Uncleared = new();

    public List<GameObject> Uncleared
    {
        get { return _Uncleared; }
        set
        {
            _Uncleared = value;
            Clear = _Uncleared.Count > 0;
        }
    }


    public void Initialise(Vector2Int coordinate)
    {
        FloorID = "Level"+PlayerPrefs.GetInt("CurrentLevel")+coordinate+gameObject.name;

        if (PlayerPrefs.HasKey(FloorID))
        {
            Clear = PlayerPrefs.GetInt(FloorID) == 1;
        }

        else
        {
            PlayerPrefs.SetInt(FloorID, 0);
        }

        if (!Clear)
        {
            foreach (IFloorObject f in transform.GetComponentsInChildren<IFloorObject>())
            {
                GameObject floorObject = f.Trigger(this);
                if (floorObject != null) { Uncleared.Add(floorObject); }
            }

            if (Uncleared.Count == 0)
            {
                Clear = true;
                PlayerPrefs.SetInt(FloorID, 1);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something Collided");
        if (other.CompareTag("Player"))
        {
            Debug.Log("His "+other.transform.position);
            Debug.Log("Mine "+this.transform.position);

            Debug.Log("Player Collided");
            InRoom = true; 
            if (!Clear) { StartCoroutine(CountDown()); }

            else
            {
                PlayerPrefs.SetFloat("PlayerX", transform.position.x);
                PlayerPrefs.SetFloat("PlayerY", other.transform.position.y);
                PlayerPrefs.SetFloat("PlayerZ", transform.position.z);
            }
        }


        
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Something Left");
        if (other.CompareTag("Player"))
        {
            InRoom = false;
        }

    }


    private IEnumerator CountDown() 
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Number of Doors = "+DoorList.Count);
        Debug.Log("Cleared " + Clear);
        Debug.Log("In Room " + InRoom);

        if (InRoom) { DoorList.ForEach(door => door.SetState(true)); }


    }

}
