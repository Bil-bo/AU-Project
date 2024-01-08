using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Enemy with unimplemented behaviour: changes based on counter
public class EnemyA : BattleEnemy
{ 
    private int counterField = 0;

    public override IEnumerator DoTurn(){

        

        // Basic pattern
        switch(counterField){
            case (0):
            
            //ActionManager.Instance.AddToBottom( new AbsorbHealth(this, new List<BaseBattleCharacter>(), 20, DamageType.NORMAL, 30));
            AttackPlayer();
            break;

            case(1):
            AttackPlayer();
            break;

            case(2):
            //ActionManager.Instance.AddToBottom( new AbsorbHealth(this, new List<BaseBattleCharacter>(), 20, DamageType.NORMAL, 30));
            AttackPlayer();
            //AttackPlayer();
            break;
            
            default:
            AttackPlayer();
            break;
        

        }
        counterField = (counterField >= 3) ? 0:counterField+1;
        
        

        yield return null;
    }



}
