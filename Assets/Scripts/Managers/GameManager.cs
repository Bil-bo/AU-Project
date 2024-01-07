using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// Somewhat Deprecated at this point, should be moved to battleManager most likely
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public RewardPanel panel;

    public GameObject overlayCanvas;
    public TMP_Text overlayText;
    private string message = "";

    void Awake()
    {
        if (instance == null) { instance = this; }
        // Hide the overlay canvas initially
        if (overlayCanvas != null)
        {
            overlayCanvas.SetActive(false);
        }
    }

    public void ExitBattle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


    public void ShowOverlay(string message)
    {
        this.message = message;
        // Show the overlay canvas with the provided message
        if (overlayCanvas != null)
        {
            overlayText.text = message;
            overlayCanvas.SetActive(true);
        }
    }
}
