using UnityEngine;
using System.Collections;

public class TurnBasedCharacter : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private HUDManager hudManager;
    public bool isPlayerTurn = false;
    private Renderer characterRenderer;
    private Material originalMaterial;

    void Start()
    {
        currentHealth = maxHealth;
        hudManager = FindFirstObjectByType<HUDManager>();
        characterRenderer = GetComponent<Renderer>();
        originalMaterial = new Material(characterRenderer.material);    
    }

    public IEnumerator TakeTurn()
    {
        isPlayerTurn = gameObject.CompareTag("Player");
        hudManager.UpdateTurnText(gameObject.name);
        UpdateHealthBar();

        if (!isPlayerTurn)
        {
            AttackRandomPlayer();
        }

        // Player's turn: Wait for input
        while (isPlayerTurn)
        {
            yield return null;
        }

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        hudManager.UpdateHealthBar(gameObject.name, currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        FlashObject(new Color(1f, 0f, 0f, 0.5f));
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log(gameObject.name + " has been defeated.");
            
            Destroy(gameObject);
        }
        UpdateHealthBar();
    }

    public void OnTargetSelected(TurnBasedCharacter targetCharacter)
    {
        if (isPlayerTurn)
        {
            int damage = 20; 
            targetCharacter.TakeDamage(damage);
            Debug.Log(gameObject.name + " does " + damage + " damage to " + targetCharacter.gameObject.name);

            // End the player's turn after the attack
            isPlayerTurn = false;
        }
    }

    void AttackRandomPlayer()
    {
        // Get all player characters in the scene
        TurnBasedCharacter[] playerCharacters = FindObjectsByType<TurnBasedCharacter>(FindObjectsSortMode.None);
        TurnBasedCharacter[] players = System.Array.FindAll(playerCharacters, player => player.CompareTag("Player"));

        if (playerCharacters.Length > 0)
        {
            // Randomly select a player character
            TurnBasedCharacter randomPlayer = players[Random.Range(0, players.Length)];

            // Attack the randomly selected player
            int damage = 5;
            randomPlayer.TakeDamage(damage);
            Debug.Log(gameObject.name + " does " + damage + " damage to " + randomPlayer.gameObject.name);
        }
    }

    void FlashObject(Color flashColor)
    {
        Material originalMaterial = characterRenderer.material;

        characterRenderer.material.color = flashColor;

        // Use Invoke to change the color back after a delay
        Invoke("ResetColor", 0.2f);
    }

    void ResetColor()
    {
        characterRenderer.material = originalMaterial;
    }
}
