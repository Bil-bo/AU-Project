using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonSort : MonoBehaviour
{
    List<GameObject> buttonObjects;
    private List<Button> buttons = new List<Button>();

    // Start is called before the first frame update
    void Start()
    {
        buttonObjects = GameObject.FindGameObjectsWithTag("Button").ToList();
        foreach (GameObject button in buttonObjects)
        {
            buttons.Add(button.GetComponent<Button>());
        }
        for (int i = 0; i < buttons.Count; i++)
        { 
            if (i == 0)
            {
                buttons[i].SetState(Button.State.Primed);
            }
            else 
            {   if (i == buttons.Count - 1) { 
                    buttons[i].SetTrigger(GameObject.Find("ButtonDoor"));
                }
                buttons[i].SetState(Button.State.inActive);
                buttons[i].SetPrev(buttons[i-1]);
                buttons[i - 1].SetNext(buttons[i]);

            }
        }
    }
}
