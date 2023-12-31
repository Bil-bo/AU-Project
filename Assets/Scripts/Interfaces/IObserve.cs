

// Partner interfaces to the IEvents, for better understanding
public interface IObserve {}


// Broadcast from the DealDamage card action
public interface IOnPreTakeDamage : IObserve
{
    void OnPreTakeDamage(PreTakeDamageEvent eventData);

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

public interface IOnTriggerBattle: IObserve
{
    void OnTriggerBattle(BattleTriggerEvent eventData);
}

public interface IOnTreasureCollected : IObserve
{
    void OnTreasureCollected(TreasureCollectedEvent eventData);
}
public interface IOnPickUpCollected : IObserve
{
    void OnPickUpCollected(PickupCollectedEvent eventData);
}

public interface IOnBossDefeated : IObserve
{
    void OnBossDefeated(BossDefeatedEvent eventData);
}

public interface IOnFinalBossDefeated : IObserve
{
    void OnFinalBossDefeated(FinalBossDefeatedEvent eventData);
}

public interface IOnLevelPassed : IObserve
{
    void OnLevelPassed(LevelPassedEvent eventData);
}
public interface IOnPlayerRescued : IObserve
{
    void OnPlayerRescued(PlayerRescuedEvent eventData);
}