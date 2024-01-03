using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveHandler : MonoBehaviour
{
    private GameObject[] enemies;
    private GameObject[] pickups;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").OrderBy(enemy => enemy.name).ToArray();
        pickups = GameObject.FindGameObjectsWithTag("Pickup").OrderBy(pickup => pickup.name).ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Save()
    {
        Debug.Log("Save");
        
        SaveData gameData = new SaveData()
        {
            enemyEnabled = new bool[enemies.Length],
            pickupEnabled = new bool[pickups.Length],
            playerPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z),
            isPuzzleComplete = GameData.Instance.isPuzzleComplete,
            pickups = PlayerPrefs.GetInt("PickupsCollected")
        };
   
        for (int i = 0; i < enemies.Length; i++)
        {
            gameData.enemyEnabled[i] = enemies[i].activeSelf;
        }

        for (int i = 0; i < pickups.Length; i++)
        {
            gameData.pickupEnabled[i] = pickups[i].activeSelf;
        }

        string jsonData = JsonUtility.ToJson(gameData);
        Debug.Log(jsonData);
        PlayerPrefs.SetString("SaveDataSlot1", jsonData);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        Debug.Log("Load");
        string jsonData = PlayerPrefs.GetString("SaveDataSlot1");
        SaveData loadedGameData = JsonUtility.FromJson<SaveData>(jsonData);

        // Use the loaded data to enable/disable enemies and pickups and set the player position
        for (int i = 0; i < enemies.Length; i++)
        {
            // Simply defaults to false if index is out of bounds (i.e the enemy did not exist when the game was saved)
            bool enableEnemy = (i >= 0 && i < loadedGameData.enemyEnabled.Length) ? loadedGameData.enemyEnabled[i] : false;
            enemies[i].SetActive(enableEnemy);
        }

        for (int i = 0; i < pickups.Length; i++)
        {
            // Same as above
            bool enablePickup = (i >= 0 && i < loadedGameData.pickupEnabled.Length) ? loadedGameData.pickupEnabled[i] : false;
            pickups[i].SetActive(enablePickup);
        }

        player.transform.position = loadedGameData.playerPosition;
        GameData.Instance.isPuzzleComplete = loadedGameData.isPuzzleComplete;
        PlayerPrefs.SetInt("PickupsCollected", loadedGameData.pickups);
        FindObjectsByType<DoorInit>(FindObjectsSortMode.None)[0].LoadDoors();
    }
}
