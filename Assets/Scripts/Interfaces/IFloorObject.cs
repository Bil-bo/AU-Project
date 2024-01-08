using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Simple interface for floor objects to get triggered by their floor on instantiation
public interface IFloorObject
{
    string ID { get; set; }


    // Pass Back The GameObject to the floorPlan Parent
    public GameObject Trigger(string floorID, int ObjectID);


}
