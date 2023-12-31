using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA : BattleEnemy
{ 
    private int counterField = 0;


    // Start is called before the first frame update

    protected override void Awake()
    {
        maxHealth = 75;
        base.Awake();
    }

    public override IEnumerator DoTurn(){

        hudManager.UpdateTurnText(gameObject.name);
        

        // Basic pattern
        switch(counterField){
            case (0):
            AttackPlayer();
            break;

            case(1):
            Defend();
            break;

            case(2):
            AttackPlayer();
            break;
            
            default:
            AttackPlayer();
            break;
        

        }
        counterField = (counterField >= 3) ? 0:counterField+1;
        
        

        yield return null;
    }


}
