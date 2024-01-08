using System.Collections;
using UnityEngine;
using TMPro;
using System;


// Base from which all battle characters inherit from
public abstract class BaseBattleCharacter : MonoBehaviour
{

    // Unique identifier
    public Guid CharID { get; } = Guid.NewGuid();
    public string Name;
    public int maxHealth;


    public int CurrentHealth; 
    public int Attack = 0;

    public int Defense = 0;
    private Renderer characterRenderer;
    public GameObject damageNumberPrefab;
    private Material originalMaterial;
    public int Position = 0;

    private GameObject _PositionMarker;
    public GameObject Targeter { get; set; }

    // Set a targeter to this scripts' gameobject
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

            if (value != null)
            {
                transform.SetParent(PositionMarker.transform, true);
                transform.localPosition = Vector3.zero;
                Targeter = value.transform.GetChild(0).gameObject;
                Targeter.transform.SetParent(transform, true);
                Targeter.transform.localPosition = transform.localPosition + new Vector3(0, 2.5f, 0);
                Position = value.GetComponent<PositionPointer>().Position;
            }
        }
    }

    public bool CanSelect { get; set; } = false;


    public virtual void Awake()
    {
        CurrentHealth = maxHealth; //Initialise health and everything else
        characterRenderer = GetComponent<Renderer>();
        originalMaterial = new Material(characterRenderer.material);

        
    }


    public abstract IEnumerator DoTurn();


    // On damage
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


    public virtual void TakeDamage(int damage) //Method for the chars to take damage
    {
        int finalDamage = Mathf.Max(damage-Defense, 0);
        CurrentHealth = Mathf.Max(0, CurrentHealth - finalDamage); //We take away a bit of health based on how much damage the char has inflicted
        FlashObject(new Color(1f, 0f, 0f, 0.5f)); //They turn red temporarily when getting attacked

        DisplayText(finalDamage.ToString());
    }

    public void RestoreHealth(int heal)
    {
        if(CurrentHealth < maxHealth)
        {
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + heal);
            FlashObject(new Color(0f, 1f, 0f, 0.5f));
        }

        DisplayText(heal.ToString());
    }
}
