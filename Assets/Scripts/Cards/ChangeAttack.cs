using System;
using UnityEditor.Timeline.Actions;
using UnityEngine;

internal class ChangeAttack : ICardActions
{

    private BaseBattleCharacter Target;
    private int Attack;

    public ChangeAttack(BaseBattleCharacter target, int newAtk)
    {
        this.Target = target;   
        this.Attack = newAtk;

    }
    public void Effect()
    {
        Target.Attack = Attack;
        AttackChangedEvent gameEvent = new AttackChangedEvent
        {
            CharacterID = Target.CharID,
            ChangedPlayer = Target,
            NewAtk = Attack,
        };

        EventManager.Broadcast(gameEvent);
    }
}