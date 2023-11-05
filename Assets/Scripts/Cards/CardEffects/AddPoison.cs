

// Simple Card Behaviour for applying poison to an enemy 
// 

using UnityEngine;

public class AddPoison : CardActions
{
    // Learnt about this too late oops
    [SerializeField]
    private int turns;
    public override void Effect(BaseBattleCharacter target, BattlePlayer user) 
    {
        target.ApplyStatusEffect(StatusFactory.Instance.createStatus<Poison>(turns));

    }
    public override string GetDescription()
    {
        return string.Format(Description, turns);
    }
}
