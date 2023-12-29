using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// For Holding StatusEffects that can effect the game characters
// Also no monobehaviour: no need for inspector and doesn't have to be attached to a GameObject
public abstract class StatusEffect: IOnStatusEffectAdded
{

    private int _Counter;

    public int Counter 
    {  
        get { return _Counter; } 
        set 
        {
            _Counter = value;
            CounterChange.Invoke(value);
        }
    }



    public string Name;
    public BaseBattleCharacter Target;
    public Action<int> CounterChange;


    public StatusEffect(int counter, BaseBattleCharacter target)
    {
        this.Counter = counter;
        this.Target = target;
    }

    public virtual void Initialise()
    {
        EventManager.AddListener<StatusEffectAddedEvent>(OnStatusEffectAdded, Target);
    }

    // Combine adds two of the same statusEffect together, so that they can stack
    public virtual void Combine(int extraTime)
    {
      
    }

    public virtual void OnStatusEffectAdded(StatusEffectAddedEvent eventData) 
    {
        if (eventData.Name == Name)
        {
            eventData.IsMerged.IsTrue = true;
            Combine(eventData.Counter);
        }
    }
}


public class PoisonEffect: StatusEffect, IOnStartOfTurn
{
    public PoisonEffect(int counter, BaseBattleCharacter target) : base(counter, target)
    {
        this.Name = "Poison";
    }

    public override void Initialise()
    {
        base.Initialise();
        EventManager.AddListener<StartOfTurnEvent>(OnStartOfTurn, Target);
    }
    public void OnStartOfTurn(StartOfTurnEvent eventData)
    {
        ActionManager.Instance.AddToBottom(new DealDamage(null, new List<BaseBattleCharacter> { Target }, Counter, DamageType.FLAT));
        Counter--;
    }

} 
