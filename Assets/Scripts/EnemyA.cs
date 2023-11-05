using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA : BattleEnemy
{ //This class isn't officialy being used yet
    private int counterField = 0;


    // Start is called before the first frame update
    public override void Start()
    {
        maxHealth = 75;
        base.Start();
        
    }

    public override IEnumerator DoTurn(){
        ProcessStatusEffects(); //Both

        hudManager.UpdateTurnText(gameObject.name);

        UpdateHealthBar();
        
        switch(counterField){
            case (0):
            Attack();
            break;

            case(1):
            Defend();
            break;

            case(2):
            Attack();
            break;
            
            default:
            Attack();
            break;
        

        }
        counterField = (counterField >= 3) ? 0:counterField+1;
        
        

        

        UpdateHealthBar();
        yield return null;
    }


}
