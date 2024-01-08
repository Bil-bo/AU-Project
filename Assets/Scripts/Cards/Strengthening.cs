using System.Collections.Generic;


// Adds extra damage to a user's next attack
public class Strengthening : Card
{
    public override void Use(BattlePlayer player, List<BaseBattleCharacter> targets)
    {
        
        ActionManager.Instance.AddToBottom(new AddStatusEffect(player, targets, new Strengthen(1)));
       
    }
    
}
