using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : CardActions
{
    public int Dmg;

    public override void Effect(BaseBattleCharacter target)
    {
        target.TakeDamage(Dmg);
        
    }
}
