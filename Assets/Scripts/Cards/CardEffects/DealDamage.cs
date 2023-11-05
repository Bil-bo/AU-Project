using UnityEngine;

public class DealDamage : CardActions
{
    [SerializeField]
    private int Dmg;

    // Simple Effect: Card does damge to target
    public override void Effect(BaseBattleCharacter target, BattlePlayer user)
    {
        target.TakeDamage(Dmg + user.attack);// + player.attack);
        
    }

    public override string GetDescription(BattlePlayer player)
    {
        return string.Format(Description, Dmg + player.attack);
    }
}
