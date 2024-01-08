using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class SaveHandler : MonoBehaviour
{
    private GameObject[] enemies;
    private GameObject[] pickups;
    public GameObject player;

    public TMP_Text saveTextOne;
    public TMP_Text saveTextTwo;
    public TMP_Text saveTextThree;

    public TMP_Text loadTextOne;
    public TMP_Text loadTextTwo;
    public TMP_Text loadTextThree;

    private void Awake()
    {
        SetDifficulty(); //SetDifficulty called here to ensure that the function is run before this scripts gameobject (PauseMenu) is disabled
    }
    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").OrderBy(enemy => enemy.name).ToArray();
        pickups = GameObject.FindGameObjectsWithTag("Pickup").OrderBy(pickup => pickup.name).ToArray();

        SetSlotText("SaveDataSlot1", saveTextOne);
        SetSlotText("SaveDataSlot2", saveTextTwo);
        SetSlotText("SaveDataSlot3", saveTextThree);

        SetSlotText("SaveDataSlot1", loadTextOne);
        SetSlotText("SaveDataSlot2", loadTextTwo);
        SetSlotText("SaveDataSlot3", loadTextThree);
    }

    private void SetSlotText(string slotKey, TMP_Text saveText)
    {
        if (PlayerPrefs.HasKey(slotKey))
        {
            string jsonData = PlayerPrefs.GetString(slotKey);
            SaveData loadedGameData = JsonUtility.FromJson<SaveData>(jsonData);
            // Extract the numeric part from the slotKey using regular expressions
            Match match = Regex.Match(slotKey, @"\d+");
            string slotNumber = match.Success ? match.Value : "Invalid";

            saveText.text = "Slot " + slotNumber + " - " + loadedGameData.difficulty + " - " + loadedGameData.pickups + " Pickups";
        }
        else
        {
            saveText.text = "New File";
        }
    }

    private void SetDifficulty()
    {
        if (PlayerPrefs.HasKey("Difficulty") && Enum.TryParse(PlayerPrefs.GetString("Difficulty"), out DifficultyType difficulty))
        {
            switch (difficulty)
            {
                case DifficultyType.EASY:
                    PlayerPrefs.SetFloat("HPMult", 0.8f);
                    PlayerPrefs.SetFloat("DMGMult", 0.8f);
                    break;
                case DifficultyType.NORMAL:
                    PlayerPrefs.SetFloat("HPMult", 1.0f);
                    PlayerPrefs.SetFloat("DMGMult", 1.0f);
                    break;
                case DifficultyType.HARD:
                    PlayerPrefs.SetFloat("HPMult", 1.3f);
                    PlayerPrefs.SetFloat("DMGMult", 1.3f);
                    break;
            }
        }
        else
        {
            //There should always be a difficulty playerpref, but if something goes wrong, will be set to normal difficulty
            PlayerPrefs.SetFloat("HPMult", 1.0f);
            PlayerPrefs.SetFloat("DMGMult", 1.0f);
        }
    }

    public void Save(string slot)
    {
        Debug.Log("Save");
        
        SaveData gameData = new SaveData()
        {
            enemyEnabled = new bool[enemies.Length],
            pickupEnabled = new bool[pickups.Length],
            playerPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z),
            pickups = PlayerPrefs.GetInt("PickupsCollected"),
            difficulty = PlayerPrefs.GetString("Difficulty")
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
        PlayerPrefs.SetString("SaveData" + slot, jsonData);
        PlayerPrefs.Save();
    }

    public void Load(string slot)
    {
        Debug.Log("Load");
        string jsonData = PlayerPrefs.GetString("SaveData" + slot);
        Debug.Log(jsonData);
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
        PlayerPrefs.SetInt("PickupsCollected", loadedGameData.pickups);
        PlayerPrefs.SetString("Difficulty", loadedGameData.difficulty);
        SetDifficulty();
        PlayerPrefs.Save();
    }
}
