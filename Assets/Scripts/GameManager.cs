using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject overlayCanvas;
    public TMP_Text overlayText;

    void Awake()
    {
        // Hide the overlay canvas initially
        if (overlayCanvas != null)
        {
            overlayCanvas.SetActive(false);
        }
    }

    void Update()
    {
        CheckWinLossConditions();
    }

    void CheckWinLossConditions()
    {
        // Check if all enemies are defeated
        if (AllEnemiesDefeated())
        {
            ShowOverlay("You Won!");
        }

        // Check if all players are defeated
        else if (AllPlayersDefeated())
        {
            ShowOverlay("You Lost!");
        }
    }

    bool AllEnemiesDefeated()
    {
        // Find all game objects with the "Enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Return true if there are no enemies left in the scene
        return enemies.Length == 0;
    }

    bool AllPlayersDefeated()
    {
        // Find all game objects with the "Player" tag
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Return true if there are no players left in the scene
        return players.Length == 0;
    }

    void ShowOverlay(string message)
    {
        // Show the overlay canvas with the provided message
        if (overlayCanvas != null)
        {
            overlayText.text = message;
            overlayCanvas.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }
}
