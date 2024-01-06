using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFloorObject
{
    string ID { get; set; }


    // Pass Back The GameObject to the floorPlan Parent
    public GameObject Trigger(FloorManager floor);


}
