using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA : BattleEnemy
{ 
    private int counterField = 0;




    // Start is called before the first frame update

    public override void Awake()
    {
        
        
        base.Awake();
    }

    public override IEnumerator DoTurn(){

        

        // Basic pattern
        switch(counterField){
            case (0):
            
            //ActionManager.Instance.AddToBottom( new AbsorbHealth(this, new List<BaseBattleCharacter>(), 20, DamageType.NORMAL, 30));
            AttackPlayer();
            Debug.Log("EFFECT IS HAPPENING");
            break;

            case(1):
            AttackPlayer();
            break;

            case(2):
            //ActionManager.Instance.AddToBottom( new AbsorbHealth(this, new List<BaseBattleCharacter>(), 20, DamageType.NORMAL, 30));
            AttackPlayer();
            Debug.Log("EFFECT IS HAPPENING");
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
