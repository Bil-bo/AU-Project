using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent
{

}


// Broadcast from the DealDamage card action
public class PreTakeDamageEvent: IEvent
{
    public DamageCalculation DmgCalc { get; set; }

}

// Broadcast from the BaseBattleCharacter TakeDamage method
public class PostTakeDamageEvent: IEvent
{
    public BaseBattleCharacter Attacker { get; set; }
    public int DmgCalc { get; set; }

}


// Broadcast from the BaseBattleCharacter TakeDamage method
public class CharacterDeathEvent: IEvent
{

}

// Broadcast from the BattleManager CheckDeaths method
public class GameOverEvent: IEvent
{

}

// Broadcast from the BaseBattleCharacter DoTurn Method
public class StartOfTurnEvent: IEvent
{
}

// Broadcast from the BaseBattleCharacter DoTurn Method
public class EndOfTurnEvent: IEvent { }


// BroadCast from the BattleManager
public class StartOfCombatEvent: IEvent { }


//BroadCast from the BattleManager
public class EndOfCombatEvent: IEvent { }


//BroadCast from the BaseBattleCharacter
public class AttackChangedEvent: IEvent { }


// Broadcast from the ActionManager
public class CardUsedEvent: IEvent { }

public class StatusEffectAddedEvent : IEvent
{
    public string Name { get; set; }
    public int Counter { get; set; }
    public BoolContainer IsMerged { get; set; } = new BoolContainer();
}

public class BoolContainer { public bool IsTrue { get; set; } = false; }
