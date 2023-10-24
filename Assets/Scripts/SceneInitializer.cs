using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public int numberOfEnemies = 3; // Number of enemies to generate
    private int enemyCount = 0;
    public int numberOfPlayers = 2; // Number of players to generate
    private int playerCount = 0;

    void Start()
    {
        GeneratePlayers();

        GenerateEnemies();
    }

    void GenerateEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Instantiate the enemy prefab
            GameObject enemy = Instantiate(enemyPrefab, new Vector3((2f*enemyCount+1), 0f, 3f), Quaternion.identity);

            // Assign a unique name to the enemy
            enemy.name = "Enemy " + (enemyCount + 1);
            enemyCount++;

            // SET OTHER PROPERTIES OR COMPONENTS FOR ENEMY HERE
        }
    }

    void GeneratePlayers()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            // Instantiate the enemy prefab
            GameObject enemy = Instantiate(playerPrefab, new Vector3(((-2f) * (playerCount + 1)), 0f, 3f), Quaternion.identity);

            // Assign a unique name to the enemy
            enemy.name = "Player " + (playerCount + 1);
            playerCount++;

            // SET OTHER PROPERTIES OR COMPONENTS FOR PLAYER HERE
        }
    }
}
