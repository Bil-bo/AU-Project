using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Card
{
    public override void Use(BattlePlayer player, List<BaseBattleCharacter> targets)
    {
        ActionManager.Instance.AddToBottom(new DealDamage(player, targets, Damage[0], DamageType.NORMAL));
    }
}
