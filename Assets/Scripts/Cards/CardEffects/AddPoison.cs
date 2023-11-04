using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPoison : CardActions
{

    public int turns;
    public override void Effect(BaseBattleCharacter target, BattlePlayer user) 
    {
        target.ApplyStatusEffect(StatusFactory.Instance.createStatus<Poison>(turns));

    }
    public override string GetDescription()
    {
        return string.Format(Description, turns);
    }
}
