using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Responsible for Holding the data of the cards
// Alternative Setup?: Scriptable Object
public class CardData : MonoBehaviour
{
    public string CardName;
    public string Description { get; set; }
    public int Range;

    public Target Targets;

    // Since Dictionaries aren't available in the inspector, two arrays, one holding the key and the other the value, are used
    // Error Prone and tacky but best available atm

    public List<GameObject> CardInput;
    public List<GameObject> CardOutput;

    public Dictionary<GameObject, GameObject> Combinations = new Dictionary<GameObject, GameObject>();

    public List<CardActions> Effects = new List<CardActions>();

    // Oh also this has to be called since awake and start aren't called because of the setup of using empty prefabs attached to real GameObjects
    public void initialise()
    {
        GameObject hold;
        if (CardInput.Count > 0 && CardInput != null)
        {
            for (int i = 0; i < CardInput.Count; i++)
            {

                if (!Combinations.TryGetValue(CardInput[i], out hold))
                {
                    Combinations.Add(CardInput[i], CardOutput[i]);
                }
            }
        }
    }

    public enum Target
    {
        Player,
        Enemy
    }
}
