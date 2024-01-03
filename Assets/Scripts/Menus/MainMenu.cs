using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

// Simple script for buttons in main menu
public class MainMenu : MonoBehaviour
{

    public bool isNewGame;
    public GameObject mainCanvas;
    public GameObject settingsCanvas;


    public void NewGame()
    {
        PlayerPrefs.SetInt("Init", 0);
        
        SceneManager.LoadScene("Difficulties");
    }

    public void ContinueGame()
    {
        PlayerPrefs.SetInt("Init", 1);
        
        SceneManager.LoadScene("Main");
    }

    public void Diffculty()
    {
        PlayerPrefs.SetInt("Init", 0);
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
    
    public void ReturnMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void IsThisThingOn() { }

    public void StartGame(string difficultyString = "NORMAL")
    {
        if (!Enum.TryParse(difficultyString, out DifficultyType difficulty))
        {
            Debug.LogError($"ArgumentException: Unable to parse '{difficultyString}' into enum {typeof(DifficultyType).Name}");
            Debug.LogWarning("Difficulty has been set to NORMAL");
            difficulty = DifficultyType.NORMAL; // Set a default value if needed
        }

        float HPMult = 1.0f;
        float DMGMult = 1.0f;

        switch (difficulty)
        {
            case DifficultyType.HARD:
                HPMult = 1.3f;
                DMGMult = 1.3f;
                break;
            case DifficultyType.NORMAL:
                HPMult = 1.0f;
                DMGMult = 1.0f;
                break;
            default:
                break;
        }


        PlayerPrefs.SetFloat("HPMult", HPMult);
        PlayerPrefs.SetFloat("DMGMUlt", DMGMult);
        PlayerPrefs.SetString("Difficulty", difficulty.ToString());
        PlayerPrefs.Save();
        SceneManager.LoadScene("Main");

    }


    public void SettingsMenu()
    {
        mainCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

}
