using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Yeah its just EnemyA again would've been cool
public class FinalBoss : BattleEnemy
{

    private int counterField = 0;
    public override IEnumerator DoTurn()
    {
        // Basic pattern
        switch (counterField)
        {
            case (0):
                AttackPlayer();
                Debug.Log("EFFECT IS HAPPENING");
                break;

            case (1):
                AttackPlayer();
                break;

            case (2):
                AttackPlayer();
                Debug.Log("EFFECT IS HAPPENING");
                //AttackPlayer();
                break;

            default:
                AttackPlayer();
                break;


        }
        counterField = (counterField >= 3) ? 0 : counterField + 1;



        yield return null;
    }
}
