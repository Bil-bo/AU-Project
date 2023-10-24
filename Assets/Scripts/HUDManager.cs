using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public TMP_Text turnText;
    public TMP_Text healthText;

    public void UpdateTurnText(string characterName)
    {
        turnText.text = characterName + "'s Turn";
    }

    public void UpdateHealthBar(string characterName, int currentHealth, int maxHealth)
    {
        healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }
}
