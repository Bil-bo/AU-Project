using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public List<GameObject> characters = new List<GameObject>();
    public int numberOfEnemies = 3; // Number of enemies to generate
    private int enemyCount = 0;
    public int numberOfPlayers = 2; // Number of players to generate
    private int playerCount = 0;

    
    public GameManager manager;

    void Start()
    {
        manager = FindAnyObjectByType<GameManager>();
        GeneratePlayers();

        GenerateEnemies();

        StartCoroutine(StartBattlePhase());
    }

    void GenerateEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Instantiate the enemy prefab
            GameObject enemy = Instantiate(enemyPrefab, new Vector3((2f*enemyCount+1), 0f, 3f), Quaternion.identity);

            // Assign a unique name to the enemy
            enemy.name = "Enemy " + (enemyCount + 1);
            enemy.GetComponent<BattleEnemy>().Position = enemyCount + 1;
            enemyCount++;

            // SET OTHER PROPERTIES OR COMPONENTS FOR ENEMY HERE
            characters.Add(enemy);
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
            characters.Add(player);
        }
    }

   IEnumerator StartBattlePhase()
    {
        bool playerWin = false;
        bool playerLose = false;
        int currentIndex = 0;
        while(!playerWin && !playerLose){
            Debug.Log("Waiting for input");
            var currentCharacter = characters[currentIndex].GetComponent<BaseBattleCharacter>();

            currentCharacter.isMyTurn = true;
            


            yield return StartCoroutine(currentCharacter.DoTurn());
            if(currentCharacter.isMyTurn == true)
            {
                continue;
            }
            yield return new WaitForSeconds(1f);
            CheckDeaths();
            

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            
    

            playerWin = (enemies.Length == 0);
            playerLose = (players.Length == 0);

            currentIndex = (currentIndex + 1) % characters.Count;  


        }

        if (playerWin)
        {
            manager.ShowOverlay("You Won!");
            manager.ExitBattle();
        }

        // Check if all players are defeated
        else if (playerLose)
        {
            manager.ShowOverlay("You Lost!");
        }



    }

    void CheckDeaths(){
        for(int i= 0; i < characters.Count; i++){

                if(characters[i].GetComponent<BaseBattleCharacter>().dead){
                    var character = characters[i];
                    characters.RemoveAt(i);
                    Destroy(character);
                }
            }


    }
}
