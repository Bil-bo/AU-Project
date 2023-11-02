using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class BaseBattleCharacter : MonoBehaviour
{

    private int maxHealth = 100;
    private int currentHealth;

    public List<StatusEffect> ActiveStatusEffects = new List<StatusEffect>();

    public bool dead { get; set;} =  false; 
    private Renderer characterRenderer;
    public GameObject damageNumberPrefab;

    public bool isMyTurn = false;

    public HUDManager hudManager;
    private Material originalMaterial;
    public int Position = 0;
    // Start is called before the first frame update
    public virtual void Start()
    {
        currentHealth = maxHealth;
        hudManager = FindFirstObjectByType<HUDManager>();
        characterRenderer = GetComponent<Renderer>();
        originalMaterial = new Material(characterRenderer.material);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Attack();

    public abstract void Defend();

    public abstract IEnumerator DoTurn();

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

    public void UpdateHealthBar()
    {
        hudManager.UpdateHealthBar(gameObject.name, currentHealth, maxHealth);
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
            
            dead = true;
        }
        UpdateHealthBar();
    }

    

    
}
