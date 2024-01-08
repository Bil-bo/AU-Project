using System.Collections.Generic;


// Merged card
// Extra damage, range and poison damage
public class PoisonSword : Card
{
    public override void Use(BattlePlayer player, List<BaseBattleCharacter> targets)
    {
        ActionManager.Instance.AddToBottom(new AddStatusEffect(player, targets, new PoisonEffect(FlatDamage[0])));
        ActionManager.Instance.AddToBottom(new DealDamage(player, targets, Damage[0], DamageType.NORMAL));
    }
}
