using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


// Dictates main behaviours of cards
// Attached as a child to a card base for maximum functionality and look
public abstract class Card : MonoBehaviour
{
    public Transform CardBase { get; set; }

    // Scriptable Object for base informatio of the card instance
    [SerializeField]
    private CardInfo _cardInfo;

  
    public GameObject CardView { get; set; }


    // For uniquely identifying all cards, for merge purposes
    public Guid CardID { get; } = Guid.NewGuid();

    private string _Name;

    // Property, to update visible name as well as text name
    public string Name 
    {
        get { return _Name; } 
        set 
        { 
            _Name = value; 
            CardBase.GetChild(0).GetComponent<TextMeshProUGUI>().text = _Name;
        } 
    }


    // Property, to update visible Description at same time
    private string _Description;

    public string Description
    {
        get { return _Description; }

        set
        {
            _Description = ConstructDescription(value);
            CardBase.GetChild(1).GetComponent<TextMeshProUGUI>().text = _Description.ToString();
        }
    }


    private int _Cost;

    public int Cost 
    { 
        get { return _Cost; } 
    
        set 
        { 
            _Cost = value;
            CardBase.GetChild(2).GetComponent<TextMeshProUGUI>().text = _Cost.ToString();
        } 
    }


    // sorts out merges into more suitable form 
    public Dictionary<string, GameObject> Merges = new();

    private CardType _CardType;

    // For event purposes
    public CardType CardType
    {
        get { return _CardType; }

        set
        {
            _CardType = value;
            CardBase.GetChild(3).GetComponent<TextMeshProUGUI>().text = _CardType.ToString();
        }
    }

    public Target Target { get; set; }

    private int _Range;
    private bool initSet = false;

    // How far the card can attack
    public int Range
    {
        get { return _Range; }

        set
        {
            if (!initSet)
            {
                _Range = Mathf.Clamp(value, -1, 10);
                initSet = true;
                if (_Range > -1) { CardBase.GetChild(4).GetComponent<TextMeshProUGUI>().text = Range.ToString(); } 
            }
            else
            {
                if (_Range != -1)
                {
                    _Range = Mathf.Max(value, 0);
                    CardBase.GetChild(4).GetComponent<TextMeshProUGUI>().text = _Range.ToString();
                }
            }
        }
    }


   // As a list to hold multiple damage values for selection purposes
    private List<int> _Damage = new();

    // Damage property to apply initial modifiers to damage on get
    public List<int> Damage { get { return _Damage.Select(item => item + DamageModifier).ToList();  } set { _Damage = value; } }

    // The current attack of the player
    private int _DamageModifier = 0;

    public int DamageModifier 
    {
        get { return _DamageModifier; }
        set 
        { 
            _DamageModifier = value;
            Description = _cardInfo.Description;

        }
    }

    protected List<int> FlatDamage;


    // For getting events with low dependency
    public Guid UserID;


    //Init card
    private void Awake()
    {
        CardView = gameObject;
        CardBase = gameObject.transform.parent;
        Name = _cardInfo.Name;
        Cost = _cardInfo.Cost;
        FormMerges(_cardInfo.CardInput, _cardInfo.CardOutput);
        CardType = _cardInfo.Type;
        Target = _cardInfo.Target;
        Range = _cardInfo.Range;
        Damage = _cardInfo.Damage;
        FlatDamage = _cardInfo.FlatDamage;
        Description = _cardInfo.Description;


    }
    
    // Dictionaries are not serialisable (incompatible with scriptable objects) so set up here
    private void FormMerges(List<string> inputs, List<GameObject> outputs)
    {
        for (int i = 0; i < inputs.Count; i++) 
        {
            Merges[inputs[i]] = outputs[i];
        }
    }

    public virtual void Selected() { }

    public virtual void Deselected() { }

    // Main override, where cards effects go
    public abstract void Use(BattlePlayer player, List<BaseBattleCharacter> targets);


    // Check If two cards can merge
    // If they can, return the card to merge
    public virtual GameObject CanMerge(Card cardToMerge) {
        if (Merges.ContainsKey(cardToMerge.Name))
        {
            return Merges[cardToMerge.Name];
        }
        else if (cardToMerge.Merges.ContainsKey(Name))
        {
            return cardToMerge.Merges[Name];
        }
        return null;
    }


    // Using substrings to create a variable description
    private string ConstructDescription(string initDescription)
    {
        string finishedDescription = initDescription;

        if (finishedDescription.Contains("|D|"))
        {

            int index = finishedDescription.IndexOf("|D|");
            int i = 0;
            // Continue replacing until no more occurrences are found
            while (i < Damage.Count && index != -1)
            {

                finishedDescription = finishedDescription.Substring(0, index) +
                    Damage[i].ToString() +
                    finishedDescription.Substring(index + "|D|".Length);

                index = finishedDescription.IndexOf("|D|", index + 1);
                i++;
            }
        }

        if (finishedDescription.Contains("|C|"))
        {
            finishedDescription = finishedDescription.Replace("|C|", (Cost == -1) ? "X" : Cost.ToString());
        }

        if (finishedDescription.Contains("|F|"))
        {
            int index = finishedDescription.IndexOf("|F|");
            int i = 0;
            // Continue replacing until no more occurrences are found
            while (i < Damage.Count && index != -1)
            {
                // Replace the current occurrence of "{DamageValue}" with the actual damage value
                finishedDescription = finishedDescription.Substring(0, index) +
                    Damage[i].ToString() +
                    finishedDescription.Substring(index + "|F|".Length);

                // Find the index of the next occurrence of "{DamageValue}"
                index = finishedDescription.IndexOf("|F|", index + 1);
                i++;

            }
        }

        return finishedDescription;
    }
}


