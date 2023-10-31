using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Button : MonoBehaviour
{

    private State state;
    private Button next;
    private Button prev;
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
                    next.SetState(State.Primed);
                }
                else
                    if (toTrigger != null)
                    {
                        toTrigger.SetActive(false);
                    }
                {
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
        }
        prev.Reset();
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
    public State getState() { return state; }

    public void SetState(State state) { this.state = state; }

    public Button getNext() { return next; }

    public void SetNext(Button next) { this.next = next; }

    public Button getPrev() { return prev; }

    public void SetPrev(Button prev) { this.prev = prev; }


    public void SetTrigger(GameObject toTrigger) { this.toTrigger = toTrigger; }
}
