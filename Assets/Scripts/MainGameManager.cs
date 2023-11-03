using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour
{
    private GameObject[] enemies;
    private GameObject[] pickups;
    public GameObject player;
    private static bool init = false;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").OrderBy(enemy => enemy.name).ToArray();
        pickups = GameObject.FindGameObjectsWithTag("Pickup").OrderBy(enemy => enemy.name).ToArray();


        if (init)
        {
            // Load the enabled state for each enemy and set it
            for (int i = 0; i < enemies.Length; i++)
            {
                int enemyEnabled = PlayerPrefs.GetInt("Enemy" + i + "Enabled");
                enemies[i].SetActive(enemyEnabled == 1);
            }
            // Load the enabled state for each pickup and set it
            for (int i = 0; i < pickups.Length; i++)
            {
                int pickupEnabled = PlayerPrefs.GetInt("Pickup" + i + "Enabled");
                pickups[i].SetActive(pickupEnabled == 1);
            }
            // Load the player's position and set it
            float playerX = PlayerPrefs.GetFloat("PlayerX");
            float playerY = PlayerPrefs.GetFloat("PlayerY");
            float playerZ = PlayerPrefs.GetFloat("PlayerZ");
            player.transform.position = new Vector3(playerX, playerY, playerZ);
            
        }
        else
        {
            init = true;
            PlayerPrefs.SetInt("PickupsCollected", 0);
        }
    }

    public void EnterBattle(List<GameObject> cards)
    {
        GameData.Instance.deckToPass = cards;
        
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);


        for (int i = 0; i < enemies.Length; i++)
        {
            PlayerPrefs.SetInt("Enemy" + i + "Enabled", enemies[i].activeSelf ? 1 : 0);
        }
        for (int i = 0; i < pickups.Length; i++)
        {
            PlayerPrefs.SetInt("Pickup" + i + "Enabled", pickups[i].activeSelf ? 1 : 0);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void Reset(Vector3 initPos)
    {
        GameData.Instance.Restart();

        PlayerPrefs.SetFloat("PlayerX", initPos.x);
        PlayerPrefs.SetFloat("PlayerY", initPos.y);
        PlayerPrefs.SetFloat("PlayerZ", initPos.z);
        PlayerPrefs.SetInt("PickupsCollected", 0);

        for (int i = 0; i < enemies.Length; i++)
        {
            PlayerPrefs.SetInt("Enemy" + i + "Enabled", 1);
        }
        for (int i = 0; i < pickups.Length; i++)
        {
            PlayerPrefs.SetInt("Pickup" + i + "Enabled", 1);
        }
    }




}
