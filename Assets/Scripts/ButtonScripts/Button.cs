using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Button : MonoBehaviour
{

    public State state { get; set; }
    public Button next { get; set; }
    public Button prev { get; set; }
    private GameObject toTrigger;
    public Material ActiveColour;
    public Material inActiveColour;
    public enum State
    {
        inActive,
        Active,
        Primed
    }
    public void collided()
    {
        switch (state)
        {
            case State.Primed:
                state = State.Active;
                if (next != null)
                {
                    next.state = State.Primed;
                }
                else
                {
                    if (toTrigger != null)
                    {
                        if (toTrigger.GetComponent<Door>() != null)
                        {
                            Debug.Log("Setting to true");
                            GameData.Instance.SetDoor(toTrigger.GetComponent<Door>(), true);
                        }
                        toTrigger.SetActive(false);

                    }
                    Complete();
                }
                break;
            case State.inActive:
                prev.Reset();
                break;
            default: break;
        }
    }

    public void Reset()
    {
        if (prev == null)
        {
            state = State.Primed;
        }
        else
        {
            state = State.inActive;
            prev.Reset();
        }

    }

    public void Complete()
    {
        Button head = this;
        Button nextDestroy = null;
        while (head.prev != null)
        {
            head = head.prev;  
        }
        while (head != this)
        {
            nextDestroy = head.next;
            head.gameObject.SetActive(false);
            head = nextDestroy;
        }
        GameData.Instance.isPuzzleComplete = true;
        this.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (state == State.Active)
        {
            GetComponent<Renderer>().material = ActiveColour;
        }

        else
        {
            GetComponent<Renderer>().material = inActiveColour;
        }
    }

    public void SetTrigger(GameObject toTrigger) { this.toTrigger = toTrigger; }
}
