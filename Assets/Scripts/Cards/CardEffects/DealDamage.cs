using System;
using System.Collections.Generic;
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
        switch (TypeOfDamage)
        {
            case DamageType.FLAT:
                foreach (BaseBattleCharacter target in Target) { target.TakeDamage(Damage); }
                break;
            default:


                foreach (BaseBattleCharacter target in Target)
                {
                    DamageCalculation calc = new();
                    PreTakeDamageEvent preDamage = new()
                    {
                        DmgCalc = calc
                    };
                    EventManager.Broadcast(preDamage, target);
                    target.TakeDamage(calc.CalculateDamage(Damage));
                    PostTakeDamageEvent postDamage = new()
                    {
                        Attacker = User,
                        DmgCalc = calc.CalculateDamage(Damage)

                    };
                    EventManager.Broadcast(postDamage, target);
                }
                break;

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
    public float ModifiedDamage;
    private List<Modifier> Modifications = new();

    public void AddModifier(Modifier modifier) { Modifications.Add(modifier); }

    public int CalculateDamage(int OriginalDamage)
    {
        ModifiedDamage = OriginalDamage;
        Modifications.Sort((a,b) => a.priority.CompareTo(b.priority));
        foreach (var modifier in Modifications) 
        {
            modifier.Modify(ModifiedDamage);
        }

        return Mathf.RoundToInt(ModifiedDamage);

    }

}
