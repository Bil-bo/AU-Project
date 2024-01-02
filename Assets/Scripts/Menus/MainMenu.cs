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

    public void StartEasy()
    {

        PlayerPrefs.SetFloat("HPMult", 0.8f);
        PlayerPrefs.SetFloat("DMGMult", 0.8f);
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
        PlayerPrefs.SetFloat("HPMult", 1.0f);
        PlayerPrefs.SetFloat("DMGMult", 1.0f);
        SceneManager.LoadScene("Main");
    }

    public void StartHard()
    {
        PlayerPrefs.SetFloat("HPMult", 1.3f);
        PlayerPrefs.SetFloat("DMGMult", 1.3f);
        SceneManager.LoadScene("Main");
    }
}
