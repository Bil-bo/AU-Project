using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpsData : MonoBehaviour
{ 
    public GameObject door;

    public void PickedUp()
    {
        this.gameObject.SetActive(false);
        
    }




}
