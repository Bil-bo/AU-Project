using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;


// For visualising the enemies health 
public class HealthBar : MonoBehaviour, IOnPostTakeDamage, IOnPostHealing
{

    // The character the health bar is listening to
    private Guid CharID;
    public int MaxHealth {  get; set; }
    public int CurrentHealth { get; set; }


    // currenthealth / maxhealth
    private float HealthPercent = 1f;


    // For moving the bar to make it appear to decrease from one end
    [SerializeField]
    Transform Bar;

    [SerializeField]
    private TextMeshPro HealthText;

    [SerializeField]
    private Material HealthMaterial;

    private Material MaterialCopy;

    // Listener handling
    private void Awake()
    {
        EventManager.AddListener<PostTakeDamageEvent>(OnPostTakeDamage);
        EventManager.AddListener<PostHealingEvent>(OnPostHealing);


        if (transform.parent != null)
        {
            transform.localPosition = transform.parent.localPosition + new Vector3(0, 1.5f, 0);
        }

    }


    // set up
    public void Initialise(Guid character, int MaxHealth, int CurrentHealth)
    {
        this.MaxHealth = MaxHealth;

        this.CurrentHealth = CurrentHealth;


        if (MaterialCopy == null)
        {
            MaterialCopy = new Material(HealthMaterial);
            Bar.GetComponent<Renderer>().material = MaterialCopy;

        }

        CharID = character;
        SetValues();
    }

    public void OnPostTakeDamage(PostTakeDamageEvent eventData)
    {
        if (CharID == eventData.DefenderID)
        {
            CurrentHealth = eventData.NewHealth;
            SetValues();
        }
        
    }


    // Set the new values
    private void SetValues()
    {
        HealthPercent = (CurrentHealth * 1.0f) / MaxHealth;

        HealthText.text = CurrentHealth.ToString() + "/" + MaxHealth.ToString();


        MaterialCopy.color = new Color(1f - HealthPercent, HealthPercent, MaterialCopy.color.b);
        Bar.localScale = new Vector3(HealthPercent, Bar.localScale.y, Bar.localScale.z);

        float offset = (1f - HealthPercent) * 0.5f;
        Bar.localPosition = new Vector3(offset, Bar.localPosition.y, Bar.localPosition.z);

    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<PostTakeDamageEvent>(OnPostTakeDamage);
        EventManager.RemoveListener<PostHealingEvent>(OnPostHealing);
        
    }

    public void OnPostHealing(PostHealingEvent eventData)
    {
        if (CharID == eventData.TargetID)
        {
            CurrentHealth = eventData.NewHealth;
            SetValues();
        }
    }

}


