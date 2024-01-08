using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;


// Simple interface for storing data from events
public interface IEvent
{

}


// Broadcast from the DealDamage card action
public class PreTakeDamageEvent: IEvent
{
    public Guid Defender { get; set; }
    
    public Guid Attacker {get; set; }
    public DamageCalculation DmgCalc { get; set; }

}


// Broadcast from absorb health
public class PreHealEvent : IEvent
{
    public Guid Defender {get; set; }
    public DamageCalculation HP {get; set; }
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

// from absorb health
public class PostHealingEvent: IEvent
{
    public Guid TargetID {get; set; }
    public BaseBattleCharacter Target { set; get; }
    public BaseBattleCharacter Healer {get; set; }

    public int HealAmount{get; set;}

    public int NewHealth {get; set;}
}


// player TakeDamage method
public class PlayerDeathEvent: IEvent
{
    public Guid ID { get; set; }
    public BattlePlayer player { get; set; }
}

// enemy take damage
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


// From the addStatusEffect Counter
public class StatusEffectAddedEvent : IEvent
{
    public Guid CharacterID { get; set; }
    public BaseBattleCharacter Target { get; set; }
    public string Name { get; set; }
    public int Counter { get; set; }
    public BoolContainer IsMerged { get; set; } = new BoolContainer();
}


/* ROAMING SCENE EVENTS */

// from the enemy roaming classes
public class BattleTriggerEvent : IEvent {}


// from treasure chests
public class TreasureCollectedEvent : IEvent
{
    public List<GameObject> Treasure { get; set; } = new();
    public PlayerPropsRoaming Collider {  get; set; }
}

// From pickups
public class PickupCollectedEvent: IEvent
{
    public PickUpsData PickUp { get; set; }
}

// From boss spawners
public class BossDefeatedEvent : IEvent {}

// From final boss
public class FinalBossDefeatedEvent : IEvent { }

// From ladder
public class LevelPassedEvent : IEvent
{
    public int MoveToLevel { get; set; }
}

// From bound player
public class PlayerRescuedEvent : IEvent
{
    public string SpawnerID { get; set; }
}

// Simple data class for allowing a bool to persist
public class BoolContainer { public bool IsTrue { get; set; } = false; }
