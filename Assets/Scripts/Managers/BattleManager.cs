using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


// Sets up and starts the main battle scene
public class BattleManager : MonoBehaviour
{
    public GameObject playerPrefab;

    //public List<GameObject> characters = new List<GameObject>();

    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();
    private int enemyCount = 0;
    public int numberOfPlayers = 2; // Number of players to generate
    private int playerCount = 0;

    
    public GameManager manager;

    void Start()
    {
        manager = FindAnyObjectByType<GameManager>();
        GeneratePlayers();
        GenerateEnemies();

        //Unlock mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(StartBattlePhase());
    }


    void GenerateEnemies()
    {
        var enemyInput = GameData.Instance.battleEnemies;


        for (int i = 0; i < enemyInput.Length; i++)
        {
            // Instantiate the enemy prefab
            GameObject enemyBody = Instantiate(enemyInput[i], new Vector3((2f*enemyCount+1), 0f, 3f), Quaternion.identity);
            //Need to add the type of enemy from enemyInput list to the enemy GameObject
            BattleEnemy enemy = enemyBody.GetComponent<BattleEnemy>();

            // Assign a unique name to the enemy
            enemy.name = "Enemy " + enemy.enemyName + (enemyCount + 1);
            var currentEnemy = enemy.GetComponent<BattleEnemy>();
            currentEnemy.Position = enemyCount +1;
            currentEnemy.attack = enemy.attack;
            currentEnemy.maxHealth = enemy.maxHealth;
            currentEnemy.currentHealth = enemy.maxHealth;
            enemyCount++;

            // SET OTHER PROPERTIES OR COMPONENTS FOR ENEMY HERE
            enemies.Add(enemyBody);
        }
    }


    void GeneratePlayers()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            // Instantiate the enemy prefab
            GameObject player = Instantiate(playerPrefab, new Vector3(((-2f) * (playerCount + 1)), 0f, 3f), Quaternion.identity);

            // Assign a unique name to the enemy
            player.name = "Player " + (playerCount + 1);
            player.GetComponent<BattlePlayer>().Position = playerCount + 1;
            playerCount++;

            // SET OTHER PROPERTIES OR COMPONENTS FOR PLAYER HERE
            players.Add(player);
        }
    }

   IEnumerator StartBattlePhase()
    {
        bool playerWin = false;
        bool playerLose = false;
        while(!playerWin & !playerLose){

            for(int i=0; i<players.Count;i++)
            {
                var currentPlayer = players[i].GetComponent<BattlePlayer>();
                yield return StartCoroutine(currentPlayer.DoTurn());
                yield return new WaitForSeconds(1f);
                CheckEnemyDeaths();
                CheckPlayerDeaths();
            

    
            
            }
            
            
            for(int i=0; i<enemies.Count;i++)
            {
                var currentEnemy = enemies[i].GetComponent<BattleEnemy>();
                yield return StartCoroutine(currentEnemy.DoTurn());
                yield return new WaitForSeconds(1f);
                CheckPlayerDeaths();
                CheckEnemyDeaths();
                

    
            
            }
             
            playerWin = (enemies.Count == 0);
            playerLose = (players.Count == 0);
              


        }

        if (playerWin)
        {
            manager.ShowOverlay("You Won!");
            yield return new WaitForSeconds(2f);
            manager.ExitBattle();
        }

        // Check if all players are defeated
        else if (playerLose)
        {
            manager.ShowOverlay("You Lost!");
        }



    }

    void CheckPlayerDeaths(){
        for(int i= 0; i < players.Count; i++){

                if(players[i].GetComponent<BattlePlayer>().dead){
                    var player = players[i];
                    players.RemoveAt(i);
                    Destroy(player);
                }
            }


    }

    void CheckEnemyDeaths()
    {
        for(int i=0; i<enemies.Count;i++)
            {
                if(enemies[i].GetComponent<BattleEnemy>().dead){
                    var enemy= enemies[i];
                    enemies.RemoveAt(i);
                    Destroy(enemy);
                }
                

    
            
            }
    }
}
