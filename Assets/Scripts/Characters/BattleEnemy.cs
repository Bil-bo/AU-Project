using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEnemy : BaseBattleCharacter
{
    [SerializeField]
    public string EnemyName { get; set; }


    public virtual void AttackPlayer() {

        BattlePlayer[] players = FindObjectsByType<BattlePlayer>(FindObjectsSortMode.None); //Attack method for enemy

        if (players.Length > 0) //If we find a player 
        {
            // Randomly select a player character
            BattlePlayer randomPlayer = players[Random.Range(0, players.Length)];

            ActionManager.Instance.AddToBottom(new DealDamage(this, new List<BaseBattleCharacter> { randomPlayer }, Attack, DamageType.NORMAL));

        }

    }

    public virtual void Defend(){
       //Empty for now as we have not implemented a defense mechanism for enemies yet 
    }

    public override IEnumerator DoTurn(){




        AttackPlayer(); //Enemies can only attack on their turn
        

     
        yield return null;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (CurrentHealth == 0)
        {
            EnemyDeathEvent enemyDied = new EnemyDeathEvent()
            {
                ID = CharID,
                enemy = this
            };

            EventManager.Broadcast(enemyDied);
        }
    }

    



}
