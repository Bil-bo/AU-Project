using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    public string CardName;
    public string Description;
    public int Range;
    public Target Targets;

    public Dictionary<Card, Card> Combinations;

    public List<CardActions> Effects = new List<CardActions>();

    public enum Target
    {
        Player,
        Enemy
    }
}
