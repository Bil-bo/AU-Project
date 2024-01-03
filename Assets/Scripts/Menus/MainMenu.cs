using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Simple script for buttons in main menu
public class MainMenu : MonoBehaviour
{

    public bool isNewGame;
    public GameObject mainCanvas;
    public GameObject settingsCanvas;


    public void NewGame()
    {
        isNewGame = true;
        SceneManager.LoadScene("Difficulties");
    }

    public void ContinueGame()
    {
        isNewGame = false;
        SceneManager.LoadScene("Main");
    }

    public void Diffculty()
    {
        SceneManager.LoadScene("Difficulties");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

    public void HowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }
    
    public void returnMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    //NOTE - MOVED DIFFICULTY MODIFIERS TO SaveHandler.cs
    public void StartEasy()
    {
        PlayerPrefs.SetString("Difficulty", "Easy");
        SceneManager.LoadScene("Main");
    }

    public void SettingsMenu()
    {
        mainCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void StartMedium()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        PlayerPrefs.SetString("Difficulty", "Medium");
        SceneManager.LoadScene("Main");
    }

    public void StartHard()
    {
        PlayerPrefs.SetString("Difficulty", "Hard");
        SceneManager.LoadScene("Main");
    }
}
