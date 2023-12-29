using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public abstract class BaseBattleCharacter : MonoBehaviour, IBroadCastEvent
{
    public string Name;
    public int maxHealth;
    private int currentHealth;
    public int attack;
    public int defense;
    public bool dead { get; set;} =  false; 
    private Renderer characterRenderer;
    public GameObject damageNumberPrefab;

    public HUDManager hudManager;
    private Material originalMaterial;
    public int Position = 0;
    // Start is called before the first frame update

    private GameObject _PositionMarker;
    public GameObject Targeter { get; set; }

    public GameObject PositionMarker 
    { 
        get { return _PositionMarker; }
        set
        {
            if (_PositionMarker != null) 
            {
                Targeter.transform.SetParent(PositionMarker.transform, true);
                Targeter.transform.localPosition = Vector3.zero;
                Targeter.SetActive(false);
            }
            _PositionMarker = value;
            transform.SetParent(PositionMarker.transform, true);
            transform.localPosition = Vector3.zero;
            Targeter = value.transform.GetChild(0).gameObject;  
            Targeter.transform.SetParent(transform, true);
            Targeter.transform.localPosition = transform.localPosition + new Vector3(0, 2.0f, 0);
            Position = value.GetComponent<PositionPointer>().Position;
        }
    }

    public bool CanSelect { get; set; } = false;


    public virtual void Start()
    {
        currentHealth = maxHealth; //Initialise health and everything else
        hudManager = FindFirstObjectByType<HUDManager>();
        characterRenderer = GetComponent<Renderer>();
        originalMaterial = new Material(characterRenderer.material);

        
    }


    public abstract IEnumerator DoTurn();

    public void ProcessStatusEffects()
    {
    }

    public void ApplyStatusEffect()
    {
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

        return Mathf.FloorToInt(finalDamage);
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

    public void TakeFlatDamage(int damage)
    {

    }
}
