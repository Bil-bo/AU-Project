using System.Collections.Generic;


// card that adds crippled status to a target
public class Crippling : Card
{
    public override void Use(BattlePlayer player, List<BaseBattleCharacter> targets)
    {
        
        ActionManager.Instance.AddToBottom(new AddStatusEffect(player, targets, new Cripple(1)));
       
    }
}


