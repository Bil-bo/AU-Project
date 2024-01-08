using System.Collections.Generic;


// Short range, high damage attack
public class Sword : Card
{
    public override void Use(BattlePlayer player, List<BaseBattleCharacter> targets)
    {
        ActionManager.Instance.AddToBottom(new DealDamage(player, targets, Damage[0], DamageType.NORMAL));
    }
}
