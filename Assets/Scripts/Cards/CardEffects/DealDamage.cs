using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : CardActions
{
    public int Dmg;


    public override void Effect(BaseBattleCharacter target, BattlePlayer user)
    {
        // BattlePlayer player = gameObject.transform.parent.GetComponent<Card>().Player;
        target.TakeDamage(Dmg + user.attack);// + player.attack);
        
    }

    public override string GetDescription(BattlePlayer player)
    {
        return string.Format(Description, Dmg + player.attack);
    }
}
