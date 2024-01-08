using System.Collections.Generic;

// Poisons an enemy
public class Poison : Card
{
    public override void Use(BattlePlayer player, List<BaseBattleCharacter> targets)
    {
        ActionManager.Instance.AddToBottom(new AddStatusEffect(player, targets, new PoisonEffect(FlatDamage[0])));
    }
}
