using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// Simple Initialisation of a button puzzle
// Pseudo random (decided by load order) bc I thought the fisher-yates shuffle was too complicated when I made this
public class ButtonSort : MonoBehaviour
{
    List<GameObject> buttonObjects;
    private List<ButtonPad> buttons = new List<ButtonPad>();

    // Start is called before the first frame update
    void Start()
    {

        buttonObjects = GameObject.FindGameObjectsWithTag("Button").ToList();
        foreach (GameObject button in buttonObjects)
        {
            buttons.Add(button.GetComponent<ButtonPad>());
        }
        for (int i = 0; i < buttons.Count; i++)
        { 
            // Sets the head to primed
            if (i == 0)
            {
                buttons[i].state = ButtonPad.State.Primed;
            }
            else 
            {   
                // Ties the tail to the appropriate door
                if (i == buttons.Count - 1) {
                    buttons[i].SetTrigger(GameObject.Find("ButtonDoor"));
                }
                buttons[i].state = ButtonPad.State.inActive;
                buttons[i].prev = buttons[i-1];
                buttons[i - 1].next = buttons[i];

            }
        }
        // Deletes the puzzle if its already been done
        if (GameData.Instance.isPuzzleComplete)
        {
            buttons[buttons.Count - 1].Complete();
        }
    }
}
