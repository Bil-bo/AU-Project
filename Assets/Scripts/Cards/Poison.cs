using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Card
{
    public override void Use(BattlePlayer player, List<BaseBattleCharacter> targets)
    {
        ActionManager.Instance.AddToBottom(new AddStatusEffect(player, targets, new PoisonEffect(FlatDamage[0])));
    }
}
