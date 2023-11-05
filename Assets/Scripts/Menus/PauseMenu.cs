using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Simple Pause Menu
public class PauseMenu : MonoBehaviour
{
    public MainGameManager gameManager;
    public void ResumeGame()
    {
        Time.timeScale = 1;
        //Re-lock mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        this.gameObject.SetActive(false);   
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        gameManager.Reset(new Vector3(4,4,4));
        SceneManager.LoadScene(0);
    }

 
}