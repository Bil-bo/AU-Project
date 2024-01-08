using System.Collections.Generic;


// Basic card with high range
public class Bow : Card
{ 
    public override void Use(BattlePlayer player, List<BaseBattleCharacter> targets)
    {
        ActionManager.Instance.AddToBottom(new DealDamage(player, targets, Damage[0], DamageType.NORMAL));
    }

}
