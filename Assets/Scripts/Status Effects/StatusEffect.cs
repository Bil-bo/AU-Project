using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// For Holding StatusEffects that can effect the game characters
// Also no monobehaviour: no need for inspector and doesn't have to be attached to a GameObject
public abstract class StatusEffect: IOnStatusEffectAdded
{
    public Action<int> CounterChange;
    private int _Counter;

    public int Counter 
    {  
        get { return _Counter; } 
        set 
        {
            _Counter = Mathf.Clamp(value, 0, 999);
            CounterChange?.Invoke(_Counter);
        }
    }



    public string Name;
    public BaseBattleCharacter Target;

    public BaseBattleCharacter User;



    public StatusEffect(int counter)
    {
        this.Counter = counter;
    }

    public virtual void Initialise()
    {
        EventManager.AddListener<StatusEffectAddedEvent>(OnStatusEffectAdded);
    }

    // Combine adds two of the same statusEffect together, so that they can stack
    public virtual void Combine(int extraTime)
    {
        Counter += extraTime;
      
    }

    public virtual void OnStatusEffectAdded(StatusEffectAddedEvent eventData) 
    {
        if (eventData.Name == Name && eventData.Target == Target)
        {
            eventData.IsMerged.IsTrue = true;
            Combine(eventData.Counter);
        }
    }

    public virtual void Remove()
    {
        EventManager.RemoveListener<StatusEffectAddedEvent>(OnStatusEffectAdded);
    }
}


public class PoisonEffect: StatusEffect, IOnStartOfTurn
{
    public PoisonEffect(int counter) : base(counter)
    {
        this.Name = "Poison";
    }

    public override void Initialise()
    {
        base.Initialise();
        EventManager.AddListener<StartOfTurnEvent>(OnStartOfTurn);
    }
    public void OnStartOfTurn(StartOfTurnEvent eventData)
    {
        if (eventData.Character == Target)
        {
            ActionManager.Instance.AddToBottom(new DealDamage(null, new List<BaseBattleCharacter> { Target }, Counter, DamageType.FLAT));
            Counter--;
        }
    }

    public override void Remove()
    {
        base.Remove();
        EventManager.RemoveListener<StartOfTurnEvent>(OnStartOfTurn);
    }

} 
public class Strengthen: StatusEffect, IOnPreTakeDamage
{

    public Strengthen(int counter) : base(counter)
    {
        this.Name = "Strengthen";
    }


    public override void Initialise()
    {
        base.Initialise();
        EventManager.AddListener<PreTakeDamageEvent>(OnPreTakeDamage);

    }

    public void OnPreTakeDamage(PreTakeDamageEvent eventData)
    {
        Debug.Log("Strengthen is happening");
        if(eventData.Attacker == Target.CharID)
        {
            Debug.Log("Damage is increased!");
            Modifier damageMod = new Modifier((originalDamage) => originalDamage* 1.5f, 0);
            eventData.DmgCalc.AddModifier(damageMod);
            Counter--;
        }

    }

    public override void Remove()
    {
        base.Remove();
        EventManager.RemoveListener<PreTakeDamageEvent>(OnPreTakeDamage);
    }


}

public class Cripple : StatusEffect, IOnPreTakeDamage
{

    public Cripple(int counter) : base(counter)
    {
        this.Name = "Cripple";
    }
    public override void Initialise()
    {
        base.Initialise();
        EventManager.AddListener<PreTakeDamageEvent>(OnPreTakeDamage);
        Debug.Log("Initialising cripple effect");
    

    }

    public void OnPreTakeDamage(PreTakeDamageEvent eventData) //This reduces the enemy's damage by 50% when applied to them
    {

        Debug.Log("The Pre take damage event is happening");
        
        if(eventData.Attacker == Target.CharID)
        {
            Debug.Log(eventData.Defender);
            Debug.Log(eventData.Attacker);
            Debug.Log("Modified");
            
            Modifier damageMod = new Modifier((originalDamage) => originalDamage * 0.5f,0);
            eventData.DmgCalc.AddModifier(damageMod);
            Counter--;
           

        }

    }

    

    public override void Remove()
    {
        base.Remove();
        EventManager.RemoveListener<PreTakeDamageEvent>(OnPreTakeDamage);
        
    }

}