using System;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class AbilityCard
{
    public string Name;
    public string Description;
    public Action<BattlePlayer> ExecuteAbility; // This delegate represents the action the card will take.
    public int Range = 0;
}
