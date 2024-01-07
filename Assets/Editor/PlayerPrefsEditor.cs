#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Microsoft.Win32;

public static class PlayerPrefsUtility
{
    public static void PrintAllPlayerPrefs()
    {
        
        string registryPath = "Software\\Unity\\UnityEditor\\" + Application.companyName + "\\" + Application.productName;
        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(registryPath);

        if (registryKey != null)
        {
            string allPrefs = "ALL PLAYERPREFS:\n"; 
            foreach (string key in registryKey.GetValueNames())
            {
                string value = registryKey.GetValue(key).ToString();
                allPrefs += key + " : " + value + "\n"; // Append each key-value pair to the string
            }

            Debug.Log(allPrefs); // Log all PlayerPrefs at once
        }
        else
        {
            Debug.Log("No PlayerPrefs found for this application.");
        }
    }
}

public class PlayerPrefsEditor : EditorWindow
{
    [MenuItem("Tools/Print All PlayerPrefs")]
    private static void PrintPlayerPrefs()
    {
        PlayerPrefsUtility.PrintAllPlayerPrefs();
    }
}
#endif