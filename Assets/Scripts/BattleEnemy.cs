using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEnemy : BaseBattleCharacter
{
    [SerializeField]
    public string enemyName { get; set; }


    public virtual void Attack(){

        BattlePlayer[] players = FindObjectsByType<BattlePlayer>(FindObjectsSortMode.None); //Attack method for enemy

        if (players.Length > 0) //If we find a player 
        {
            // Randomly select a player character
            BattlePlayer randomPlayer = players[Random.Range(0, players.Length)];

            // Attack the randomly selected player
            int damage = 5;
            randomPlayer.TakeDamage(damage);
            Debug.Log(gameObject.name + " does " + damage + " damage to " + randomPlayer.gameObject.name);
        }

    }

    public virtual void Defend(){
        ApplyStatusEffect(StatusFactory.Instance.createStatus<Block>(1));
       //Empty for now as we have not implemented a defense mechanism for enemies yet 
    }

    public override IEnumerator DoTurn(){
        ProcessStatusEffects(); //Both

        hudManager.UpdateTurnText(gameObject.name);

        UpdateHealthBar();

        Attack(); //Enemies can only attack on their turn
        

        

        UpdateHealthBar();
        yield return null;
    }
    



}
