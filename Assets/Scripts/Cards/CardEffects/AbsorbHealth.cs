using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbHealth : ICardActions
{
    private BaseBattleCharacter user;
    private List<BaseBattleCharacter> Target;
    private int AbsorbPercentage;
    private int Damage;

    private DamageType TypeOfDamage;
    public AbsorbHealth(BaseBattleCharacter User, List<BaseBattleCharacter> Target, int Damage,DamageType damageType,int absorbPercentage)
    {
        this.user = User;
        this.Target = Target;
        this.Damage = Damage;
        this.AbsorbPercentage = absorbPercentage;
        this.TypeOfDamage = damageType;
    }

    
    public virtual void Effect()
    {
        //Subject to change

        foreach (BaseBattleCharacter target in Target)
        {
            DamageCalculation calc = new DamageCalculation(Damage);
            PostTakeDamageEvent postDamage = new();
            PostHealingEvent postHeal = new();
            PreHealEvent preHeal = new();


            if (TypeOfDamage == DamageType.FLAT)
            {
                
                target.TakeDamage(Damage);
                
                postDamage.DefenderID = target.CharID;
                postDamage.DmgCalc = Damage;
                postDamage.NewHealth = target.CurrentHealth;

                preHeal.Defender = user.CharID;
                preHeal.HP = new DamageCalculation(Damage*(AbsorbPercentage/100));
                EventManager.Broadcast(preHeal);
                user.RestoreHealth(preHeal.HP.CalculateDamage());

                           
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
                postDamage.Attacker = user;
                postDamage.Defender = target;
                postDamage.DmgCalc= calc.CalculateDamage();
                postDamage.NewHealth = target.CurrentHealth;

                preHeal.Defender = user.CharID;
                preHeal.HP = new DamageCalculation(Mathf.Max(0, calc.CalculateDamage()*(AbsorbPercentage/100)));
                EventManager.Broadcast(preHeal);
                user.RestoreHealth(preHeal.HP.CalculateDamage());

                

            }

            postHeal.TargetID = user.CharID;
            postHeal.Target = user;
            postHeal.Healer = user;
            postHeal.HealAmount = preHeal.HP.CalculateDamage();
            postHeal.NewHealth = user.CurrentHealth; 

            EventManager.Broadcast(postHeal);

            EventManager.Broadcast(postDamage);
        }



           /* int absorbed = 20;
            user.CurrentHealth += absorbed;
            int actualAbsorbed = Mathf.Min(absorbed, target.CurrentHealth);

            Debug.Log($"User Health: {user.CurrentHealth}, Target Health Before Absorb: {target.CurrentHealth}");
        
            // Ensure that target.TakeDamage handles negative values appropriately
            target.TakeDamage(actualAbsorbed);
        
            Debug.Log($"Target Health After Absorb: {target.CurrentHealth}"); */
        

    }

    





    



    
    
    
    
}
