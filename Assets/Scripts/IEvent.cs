using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public interface IEvent
{

}


// Broadcast from the DealDamage card action
public class PreTakeDamageEvent: IEvent
{
    public Guid Defender { get; set; }
    public DamageCalculation DmgCalc { get; set; }

}

// Broadcast from the BaseBattleCharacter TakeDamage method
public class PostTakeDamageEvent: IEvent
{
    public Guid DefenderID { get; set; }

    public BaseBattleCharacter Defender {  set; get; }
    public BaseBattleCharacter Attacker { get; set; }
    public int DmgCalc { get; set; }
    public int NewHealth { get; set; }

}


// Broadcast from the BaseBattleCharacter TakeDamage method
public class PlayerDeathEvent: IEvent
{
    public Guid ID { get; set; }
    public BattlePlayer player { get; set; }
}


public class EnemyDeathEvent : IEvent
{
    public Guid ID { get; set; }
    public BattleEnemy enemy { get; set; }
}


// Broadcast from the BattleManager CheckDeaths method
public class GameOverEvent: IEvent
{
     
}

// Broadcast from the BaseBattleCharacter DoTurn Method
public class StartOfTurnEvent: IEvent
{
    public Guid CharacterID { get; set; }
    public BaseBattleCharacter Character { get; set; }
}

// Broadcast from the BaseBattleCharacter DoTurn Method
public class EndOfTurnEvent: IEvent 
{
    public Guid CharacterID { get; set; }
    public BaseBattleCharacter Character { get; set; }

}


// BroadCast from the BattleManager
public class StartOfCombatEvent: IEvent { }


//BroadCast from the BattleManager
public class EndOfCombatEvent: IEvent { }


//BroadCast from the BaseBattleCharacter
public class AttackChangedEvent: IEvent 
{
    public Guid CharacterID { get; set; }   
    public BaseBattleCharacter ChangedPlayer { get; set; }
    public int NewAtk { get; set; }
}


// Broadcast from the ActionManager
public class CardUsedEvent: IEvent 
{

    public Guid CharacterID { get; set; }
    public BattlePlayer User { get; set; }
    public Card Card { get; set; }
}

public class StatusEffectAddedEvent : IEvent
{
    public Guid CharacterID { get; set; }
    public BaseBattleCharacter Target { get; set; }
    public string Name { get; set; }
    public int Counter { get; set; }
    public BoolContainer IsMerged { get; set; } = new BoolContainer();
}

public class BoolContainer { public bool IsTrue { get; set; } = false; }
