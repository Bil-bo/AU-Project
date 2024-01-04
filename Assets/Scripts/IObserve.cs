using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserve {}
// Broadcast from the DealDamage card action
public interface IOnPreTakeDamage : IObserve
{
    void OnPreTakeDamage(IEvent eventData);

}

public interface IOnPreHealing : IObserve
{
    void OnPreHeal(IEvent eventData);
}

// Broadcast from the BaseBattleCharacter TakeDamage method
public interface IOnPostTakeDamage : IObserve
{
    void OnPostTakeDamage(PostTakeDamageEvent eventData);
}

public interface IOnPostHealing : IObserve
{
    void OnPostHealing(PostHealingEvent eventData);
}


// Broadcast from the BaseBattleCharacter TakeDamage method
public interface IOnPlayerDeath : IObserve
{
    void OnPlayerDeath(PlayerDeathEvent eventData);

}

public interface IOnEnemyDeath : IObserve
{
    void OnEnemyDeath(EnemyDeathEvent eventData);

}


// Broadcast from the BattleManager CheckDeaths method
public interface IOnGameOver : IObserve
{
    void OnGameOver(IEvent eventData);
}


// Broadcast from the BaseBattleCharacter DoTurn Method
public interface IOnStartOfTurn : IObserve
{
    void OnStartOfTurn(StartOfTurnEvent eventData);
}


// Broadcast from the BaseBattleCharacter DoTurn Method
public interface IOnEndOfTurn : IObserve 
{
    void OnEndOfTurn(IEvent eventData);
}


// BroadCast from the BattleManager
public interface IOnStartOfCombat : IObserve 
{
    void OnStartOfCombat(IEvent eventData);
}


//BroadCast from the BattleManager
public interface IOnEndOfCombat : IObserve 
{
    void OnEndOfCombat(IEvent eventData);
}


//BroadCast from the BaseBattleCharacter
public interface IOnAttackChanged : IObserve 
{
    void OnAttackChanged(AttackChangedEvent eventData);
}


// Broadcast from the ActionManager
public interface IOnCardUsed : IObserve 
{
    void OnCardUsed(IEvent eventData);
}

// Broadcast from AddStausEfect
public interface IOnStatusEffectAdded : IObserve
{
    void OnStatusEffectAdded(StatusEffectAddedEvent eventData);
}
