using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardInfo", menuName = "CardData")]
public class CardInfo : ScriptableObject
{
    [SerializeField]
    public string Name;

    [SerializeField]
    public string Description;

    [SerializeField]
    public int Cost;

    [SerializeField]
    public List<string> CardInput = new List<string>();

    [SerializeField]
    public List<GameObject> CardOutput = new List<GameObject>();

    [SerializeField]
    public CardType Type;

    [SerializeField]
    public Target Target;

    [SerializeField]
    public int Range;

    [SerializeField]
    public List<int> Damage = new();

    [SerializeField]
    public List<int> FlatDamage = new();

}


// used for highlighting purposes, some extra logic may follow
// For example, a card that hurts an enemy for x damage and makes the player defend may use ENEMY as its target but its actual functionality may use both the player and the enemy
public enum Target
{    
    NONE, // The card highlights nothing. Useful for unplayable cards or cards that target at random. 
    PLAYER, // The card highlights players in range. Highlighting changes when hovering over a valid target
    ENEMY, // The card highlights enemies in range. Highlighting changes when hovering over a valid target
    SELF, // The card highlights the current player. The highlighting doesn't change. The card just needs to be in the play area to be played
    ALL, // The card highlights everything as 'selected'. The highlighting doesn't change. The card just needs to be in the play area to be played
    ALL_ENEMIES, // The card highlights all enemies as 'selected'. The highlighting doesn't change. The card just needs to be in the play area to be played.
    ALL_PLAYERS, // The card highlights all players as 'selected'. The highlighting doesn't change. The card just needs to be in the play area to be played.
    ALL_IN_RANGE, // The card highlights all players and enemies in range as 'selected'. The highlighting doesn't change. The card just needs to be in the play area to be played
    SELF_AND_ENEMIES, // The card highlights the current player and all enemies as 'selected'. The highlighting doesn't change. The card just needs to be in the play area to be played.
    PLAYERS_IN_RANGE, // The card highlights all players in range as 'selected'
    ENEMIES_IN_RANGE, // The card highlights all enemies in range as selected.
    PLAYERS_OR_ENEMIES // The card allows for the selection of either players or enemies.
}


// Different damage types
// Normal damage is affected by multipliers, additive values
// Distinct from the actual values inside the scriptable object, since those are meant for other things
public enum DamageType
{
    NORMAL,
    FLAT
}

// Card type for hiding things in the info editor (aka skill cards will never need to see damage values
public enum CardType
{
    ATTACK,
    SKILL,
    BUFF
}
