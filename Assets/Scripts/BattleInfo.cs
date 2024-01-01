using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Battle", menuName = "BattleData")] 
public class BattleInfo : ScriptableObject
{
    [SerializeField]
    public GameObject PlayerModel;

    [SerializeField]
    public string Name;

    [SerializeField]
    public int maxHealth;

    [SerializeField]
    public int MaxEnergy;

    [SerializeField]
    public int MaxHandSize;

    [SerializeField]
    public List<GameObject> Deck;




}
