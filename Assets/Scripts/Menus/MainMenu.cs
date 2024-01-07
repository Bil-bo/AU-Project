using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Simple script for buttons in main menu
public class MainMenu : MonoBehaviour
{

    public bool isNewGame;
    public GameObject mainCanvas;
    public GameObject settingsCanvas;

    IEnumerator Start()
    {
        AsyncOperationHandle<IList<GameObject>> loadAllHandle = Addressables.LoadAssetsAsync<GameObject>("PreLoad", null);

        yield return loadAllHandle;

        if (loadAllHandle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<GameObject> allLoadedAssets = loadAllHandle.Result;
            foreach (var asset in allLoadedAssets)
            {
                Debug.Log("Loaded Asset: " + asset.name);
            }
        }
        else
        {
            Debug.LogError("Failed to load addressables.");
        }

        Addressables.Release(loadAllHandle);
    }


    public void NewGame()
    {
        string buildPath = PlayerPrefs.GetString(Addressables.kAddressablesRuntimeDataPath);
        string logPath = PlayerPrefs.GetString(Addressables.kAddressablesRuntimeBuildLogPath);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString(Addressables.kAddressablesRuntimeDataPath, buildPath);
        PlayerPrefs.SetString(Addressables.kAddressablesRuntimeBuildLogPath, logPath);
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
            case DifficultyType.EASY:
                HPMult = 0.8f;
                DMGMult = 0.8f;
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
