using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class DealDamage : ICardActions
{

    private BaseBattleCharacter User;
    private List<BaseBattleCharacter> Target;
    private int Damage;
    private DamageType TypeOfDamage;

    public DealDamage(BaseBattleCharacter user, List<BaseBattleCharacter> targets, int damage, DamageType damageType)
    {
        this.User = user;
        this.Target = targets;
        this.Damage = damage;
        this.TypeOfDamage = damageType;

    }

    // Simple Effect: Card does damge to target
    public void Effect()
    {
        foreach (BaseBattleCharacter target in Target)
        {
            DamageCalculation calc = new DamageCalculation(Damage);
            PostTakeDamageEvent postDamage = new();


            if (TypeOfDamage == DamageType.FLAT)
            {
                
                target.TakeDamage(Damage);
                postDamage.DefenderID = target.CharID;
                postDamage.DmgCalc = Damage;
                postDamage.NewHealth = target.CurrentHealth;            
            }

            else
            {
                PreTakeDamageEvent preDamage = new()
                {
                    Defender = target.CharID,
                    DmgCalc = calc
                };
                EventManager.Broadcast(preDamage);
                target.TakeDamage(calc.CalculateDamage());

                postDamage.DefenderID = target.CharID;
                postDamage.Attacker = User;
                postDamage.Defender = target;
                postDamage.DmgCalc = calc.CalculateDamage();
                postDamage.NewHealth = target.CurrentHealth;

            }

            EventManager.Broadcast(postDamage);
        }
    }
}

public class Modifier
{
    public Func<float, float> Modify { get; set; }
    public int priority { get; set; }

    public Modifier(Func<float, float> modification, int priority)
    {
        Modify = modification;
        this.priority = priority;
    }

    

}

public class DamageCalculation
{
    public int OriginalDamage;
    public float ModifiedDamage;
    private List<Modifier> Modifications = new();

    public DamageCalculation(int originalDamage)
    {
        OriginalDamage = originalDamage;
        ModifiedDamage = originalDamage;
    }

    public void AddModifier(Modifier modifier) { Modifications.Add(modifier); }

    public int CalculateDamage()
    {
        Modifications.Sort((a,b) => a.priority.CompareTo(b.priority));
        foreach (var modifier in Modifications) 
        {
           ModifiedDamage = modifier.Modify(ModifiedDamage);
        }
        return Mathf.RoundToInt(ModifiedDamage);

    }

}
