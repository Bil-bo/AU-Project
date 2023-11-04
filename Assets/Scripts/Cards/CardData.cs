using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardData : MonoBehaviour
{
    public string CardName;
    public string Description { get; set; }
    public int Range;
    public Target Targets;

    public List<GameObject> CardInput;
    public List<GameObject> CardOutput;

    public Dictionary<GameObject, GameObject> Combinations = new Dictionary<GameObject, GameObject>();

    public List<CardActions> Effects = new List<CardActions>();

    public void initialise()
    {
        GameObject hold;
        Debug.Log("We Editing");
        if (CardInput.Count > 0 && CardInput != null)
        {
            for (int i = 0; i < CardInput.Count; i++)
            {

                if (!Combinations.TryGetValue(CardInput[i], out hold))
                {
                    Combinations.Add(CardInput[i], CardOutput[i]);
                    Debug.Log(Combinations[CardInput[i]].name + "It's Here");
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
