using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public abstract class BaseBattleCharacter : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;
    public int attack;
    public int defense;

    public Dictionary<string, StatusEffect> ActiveStatusEffects = new Dictionary<string, StatusEffect>();

    public bool dead { get; set;} =  false; 
    private Renderer characterRenderer;
    public GameObject damageNumberPrefab;

    //public bool isMyTurn = false;

    public HUDManager hudManager;
    private Material originalMaterial;
    public int Position = 0;
    // Start is called before the first frame update
    public virtual void Start()
    {
        currentHealth = maxHealth; //Initialise health and everything else
        hudManager = FindFirstObjectByType<HUDManager>();
        characterRenderer = GetComponent<Renderer>();
        originalMaterial = new Material(characterRenderer.material);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract IEnumerator DoTurn();

    public void ProcessStatusEffects()
    {
        List<string> keysToRemove = new List<string>();
        foreach (StatusEffect effect in ActiveStatusEffects.Values)
        {
            if (effect.countDown(this) <= 0)
            {
                keysToRemove.Add(effect.name);
            }
        }

        foreach (string key in keysToRemove) 
        {
            ActiveStatusEffects.Remove(key);
        
        }

    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        if (ActiveStatusEffects.ContainsKey(effect.name))
        {
            ActiveStatusEffects[effect.name].Combine(effect);
        }
        else
        {
            ActiveStatusEffects.Add(effect.name, effect); //The status effects for the characters are applied here
        }
    }

    public void UpdateHealthBar()
    {
        hudManager.UpdateHealthBar(gameObject.name, currentHealth, maxHealth); //Visual update for HP
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

    public void DisplayText(string text)
    {
        GameObject textPopup = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
        textPopup.GetComponent<TMP_Text>().text = text;
        textPopup.transform.position = new Vector3(textPopup.transform.position.x, textPopup.transform.position.y + 1f, textPopup.transform.position.z - 1f);
    }

    public int CalculateIncomingDamage(float originalDamage)
    {
        float finalDamage = originalDamage;

        return Mathf.FloorToInt(finalDamage / (ActiveStatusEffects.ContainsKey("Block") ? 4: 1));
    }

    public void TakeDamage(int damage) //Method for the chars to take damage
    {
        damage = CalculateIncomingDamage(damage);
        currentHealth -= damage; //We take away a bit of health based on how much damage the char has inflicted
        FlashObject(new Color(1f, 0f, 0f, 0.5f)); //They turn red temporarily when getting attacked

        DisplayText(damage.ToString());


        if (currentHealth <= 0)
        {
            currentHealth = 0; //Killing off characters
            Debug.Log(gameObject.name + " has been defeated.");
            
            dead = true; 
        }
        UpdateHealthBar();
    }

    

    
}
