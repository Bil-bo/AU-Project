using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Simple concept of blocking off areas before the player completes a task
public class Door : MonoBehaviour
{
    [SerializeField]
    private GameObject realDoor;

    public void SetState(bool state)
    {
        realDoor.SetActive(state);
    }
}
