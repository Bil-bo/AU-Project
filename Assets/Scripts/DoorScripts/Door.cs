using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Simple concept of blocking off areas before the player completes a task
public class Door : MonoBehaviour
{
    public string ID { get; set; }
    public bool isOpen {  get; set; }
    public void Initialise(string ID, bool isOpen)
    {
        this.ID = ID;
        this.isOpen = isOpen;
    }
}
