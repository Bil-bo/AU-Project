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

        if (!Clear)
        {
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
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

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

        if (other.CompareTag("Player"))
        {
            InRoom = false;
        }

    }


    private IEnumerator CountDown() 
    {
        yield return new WaitForSeconds(2f);

        if (InRoom) { DoorList.ForEach(door => door.SetState(true)); }


    }

}
