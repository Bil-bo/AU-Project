using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class TurnBasedCharacter : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private HUDManager hudManager;
    public bool isPlayerTurn = false;
    private Renderer characterRenderer;
    private Material originalMaterial;
    public GameManager manager;
    public bool attackPressed = false;
    public GameObject damageNumberPrefab;

    // Card Buttons
    private GameObject Card1;
    private GameObject Card2;

    //Status effect List
    public List<StatusEffect> ActiveStatusEffects  = new List<StatusEffect>();

    // Card Lists
    private List<AbilityCard> originalDeck = new List<AbilityCard>();
    public List<AbilityCard> Deck = new List<AbilityCard>();
    public List<AbilityCard> CurrentHand = new List<AbilityCard>(2); // only two abilities per turn

    public System.Action<TurnBasedCharacter> currentSelectedAbilityAction;
    public AbilityCard currentCard;

    public int Position = 0;

    void Start()
    {
        currentHealth = maxHealth;
        hudManager = FindFirstObjectByType<HUDManager>();
        characterRenderer = GetComponent<Renderer>();
        manager = FindAnyObjectByType<GameManager>();

        // Find Card Buttons
        Card1 = GameObject.Find("Card1");
        Card2 = GameObject.Find("Card2");
        originalMaterial = new Material(characterRenderer.material);

        // Create deck
        InitializeDeck();
    }

    public void ProcessStatusEffects()
    {
        for (int i = ActiveStatusEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = ActiveStatusEffects[i];
            effect.Duration--;
            if (effect.Duration <= 0)
            {
                // If the effect duration is over, remove it from the list
                ActiveStatusEffects.RemoveAt(i);
            }
        }
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        ActiveStatusEffects.Add(effect);
    }

    public void InitializeDeck()
    {
        Deck.Add(new AbilityCard
        {
            Name = "Melee Attack",
            Description = "Deal damage to the opponent.",
            Range = 1,
            ExecuteAbility = (attacker) =>
            {
                attacker.currentSelectedAbilityAction = (selectedTarget) =>
                {
                    int damage = 40; // Example damage value
                    selectedTarget.TakeDamage(damage);
                    Debug.Log(attacker.gameObject.name + " does " + damage + " damage to " + selectedTarget.gameObject.name);

                    attacker.isPlayerTurn = false; // End turn after attack
                };
            }
        });

        Deck.Add(new AbilityCard
        {
            Name = "Ranged Attack",
            Description = "Deal damage to the opponent.",
            Range = 4,
            ExecuteAbility = (attacker) =>
            {
                attacker.currentSelectedAbilityAction = (selectedTarget) =>
                {
                    int damage = 20; // Example damage value
                    selectedTarget.TakeDamage(damage);
                    Debug.Log(attacker.gameObject.name + " does " + damage + " damage to " + selectedTarget.gameObject.name);

                    attacker.isPlayerTurn = false; // End turn after attack
                };
            }
        });

        Deck.Add(new AbilityCard
        {
            Name = "Defend",
            Description = "Halve incoming damage next turn.",
            ExecuteAbility = (attacker) =>
            {
                attacker.currentSelectedAbilityAction = null;
                DisableAllButtons();
                attacker.ApplyStatusEffect(new StatusEffect
                {
                    Name = "Defensive Stance",
                    Duration = 1,
                    EffectType = EffectType.IncreaseDefense,
                    EffectValue = 2f //Defense multiplier
                });
                Debug.Log("Defend Pressed");
                DisplayText("Defense Up");
                attacker.isPlayerTurn = false;
            }
        });

        // ADD NEW CARDS HERE

        originalDeck = new List<AbilityCard>(Deck); // Store a copy of the original deck.
    }

    public void UpdateButtonAbilities()
    {
        EnableAllButtons();

        // Gets two random abilities
        DealAbilities();

        // Sets the text object of the card buttons to the name of the abilities 
        Card1.transform.Find("Text").GetComponent<TMP_Text>().text = CurrentHand[0].Name;
        Card2.transform.Find("Text").GetComponent<TMP_Text>().text = CurrentHand[1].Name;

        // Assign the abilities to the button's onClick events
        Card1.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        Card1.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => UseAbility(CurrentHand[0]));

        Card2.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        Card2.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => UseAbility(CurrentHand[1]));
    }

    private void DisableAllButtons()
    {
        Card1.GetComponent<UnityEngine.UI.Button>().interactable = false;
        Card2.GetComponent<UnityEngine.UI.Button>().interactable = false;
    }

    private void EnableAllButtons()
    {
        Card1.GetComponent<UnityEngine.UI.Button>().interactable = true;
        Card2.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }

    public void UseAbility(AbilityCard card)
    {
        card.ExecuteAbility(this);
        currentCard = card;
    }

    public void DealAbilities()
    {
        Deck = new List<AbilityCard>(originalDeck); // Reset the deck to its original state.
        CurrentHand.Clear();

        if (Deck.Count < 2)
        {
            Debug.LogError("Not enough abilities in the deck to deal!");
            return;
        }

        // Randomly select two cards
        for (int i = 0; i < 2; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, Deck.Count);
            CurrentHand.Add(Deck[randomIndex]);
            Deck.RemoveAt(randomIndex);
        }
    }

    public IEnumerator TakeTurn()
    {
        ProcessStatusEffects();
        UpdateButtonAbilities();
        isPlayerTurn = gameObject.CompareTag("Player");
        hudManager.UpdateTurnText(gameObject.name);
        UpdateHealthBar();

        if (!isPlayerTurn)
        {
            AttackRandomPlayer();
        }
        Card1.SetActive(isPlayerTurn);
        Card2.SetActive(isPlayerTurn);

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
        damage = CalculateIncomingDamage(damage);
        currentHealth -= damage;
        FlashObject(new Color(1f, 0f, 0f, 0.5f));

        DisplayText(damage.ToString());


        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log(gameObject.name + " has been defeated.");
            
            Destroy(gameObject);
            CheckWinLossConditions();
        }
        UpdateHealthBar();
    }

    public void DisplayText(string text)
    {
        GameObject textPopup = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
        textPopup.GetComponent<TMP_Text>().text = text;
        textPopup.transform.position = new Vector3(textPopup.transform.position.x, textPopup.transform.position.y + 1f, textPopup.transform.position.z - 1f);
    }

    public int CalculateIncomingDamage(float originalDamage)
    {
        float finalDamage = originalDamage;

        foreach (var effect in ActiveStatusEffects)
        {
            if (effect.EffectType == EffectType.IncreaseDefense)
            {
                finalDamage /= effect.EffectValue; // Divide damage by defense multiplier
            }
            // Add other damage modifiers here
        }

        return Mathf.FloorToInt(finalDamage);
    }

    public void OnTargetSelected(TurnBasedCharacter targetCharacter)
    {
        if (isPlayerTurn && currentSelectedAbilityAction != null)
        {
            currentSelectedAbilityAction.Invoke(targetCharacter);
            currentSelectedAbilityAction = null; // Clear the action after it's used
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

    bool AllEnemiesDefeated()
    {
        // Find all game objects with the "Enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // Return true if there are no enemies left in the scene
        return enemies.Length - 1 == 0;
    }

    bool AllPlayersDefeated()
    {
        // Find all game objects with the "Player" tag
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Return true if there are no players left in the scene
        return players.Length - 1 == 0;
    }

    void CheckWinLossConditions()
    {
        // Check if all enemies are defeated
        if (AllEnemiesDefeated())
        {
            manager.ShowOverlay("You Won!");
            manager.ExitBattle();
        }

        // Check if all players are defeated
        else if (AllPlayersDefeated())
        {
            manager.ShowOverlay("You Lost!");
        }
    }
}
