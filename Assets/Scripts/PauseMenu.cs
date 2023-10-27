using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public MainGameManager gameManager;
    public void ResumeGame()
    {
        Time.timeScale = 1;
        this.gameObject.SetActive(false);   
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        gameManager.Reset(new Vector3(4,4,4));
        SceneManager.LoadScene(0);
    }

 
}