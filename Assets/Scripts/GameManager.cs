using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject overlayCanvas;
    public TMP_Text overlayText;
    private string message = "";

    void Awake()
    {
        // Hide the overlay canvas initially
        if (overlayCanvas != null)
        {
            overlayCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        ShowOverlay(message);
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
