using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemy : BaseBattleCharacter
{

    private int atk; 
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack(){

        BattlePlayer[] players = FindObjectsByType<BattlePlayer>(FindObjectsSortMode.None);

        if (players.Length > 0)
        {
            // Randomly select a player character
            BattlePlayer randomPlayer = players[Random.Range(0, players.Length)];

            // Attack the randomly selected player
            int damage = 5;
            randomPlayer.TakeDamage(damage);
            Debug.Log(gameObject.name + " does " + damage + " damage to " + randomPlayer.gameObject.name);
        }

    }

    public override void Defend(){



    }

    public override IEnumerator DoTurn(){
        ProcessStatusEffects(); //Both

        UpdateHealthBar();

        Attack();
        isMyTurn = false;

        

        UpdateHealthBar();
        yield return null;
    }
    



}
