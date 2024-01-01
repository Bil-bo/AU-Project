using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Simple Puzzle Object With Linked List Behaviour
// Showing off concept of puzzles in the main game
public class ButtonPad : MonoBehaviour
{
    public Material ActiveColour;
    public Material inActiveColour;
    public State state { get; set; }

    // The next and previous button pads in the sequence
    public ButtonPad next { get; set; } 
    public ButtonPad prev { get; set; }

    private GameObject toTrigger;

    public enum State
    {
        // Inactive - This button is not the next to be activated
        // Active - This button has or was pressed correctly
        // Primed - This button is next in the sequence

        inActive,
        Active,
        Primed
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


    // Dynamic Behaviour for when the player presses on a button
    public void collided()
    {
        switch (state)
        {
            case State.Primed:
                state = State.Active;
                
                // If the button has no next, it is the last in the sequence and the puzzle is therefore complete
                if (next != null)
                {
                    next.state = State.Primed;
                }
                else
                {
                    // If the Tail activates something, do it here
                    if (toTrigger != null)
                    {
                        if (toTrigger.GetComponent<Door>() != null)
                        {
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


    // Setting all buttons back to their original states
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

    // For Permanently removing all buttons from the game once the puzzle is complete
    public void Complete()
    {
        ButtonPad head = this;
        ButtonPad nextDestroy = null;
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
        
        // The object that called complete has to be last to be destroyed, otherwise weird behaviour can occur
        this.gameObject.SetActive(false);

    }


// Before Learning about {get; set;} :( Allows external programs to set the state of the object
    public void SetTrigger(GameObject toTrigger) { this.toTrigger = toTrigger; }
}
