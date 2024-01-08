using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Card : MonoBehaviour
{
    public Transform CardBase { get; set; }

    [SerializeField]
    private CardInfo _cardInfo;

    public GameObject CardView { get; set; }

    public Guid CardID { get; } = Guid.NewGuid();

    private string _Name;

    public string Name 
    {
        get { return _Name; } 
        set 
        { 
            _Name = value; 
            CardBase.GetChild(0).GetComponent<TextMeshProUGUI>().text = _Name;
        } 
    }

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

    public Dictionary<string, GameObject> Merges = new();

    private CardType _CardType;

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


    private List<int> _Damage = new();


    public List<int> Damage { get { return _Damage.Select(item => item + DamageModifier).ToList();  } set { _Damage = value; } }

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



    public Guid UserID;

    //private bool Activated = false;

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

    private void FormMerges(List<string> inputs, List<GameObject> outputs)
    {
        for (int i = 0; i < inputs.Count; i++) 
        {
            Merges[inputs[i]] = outputs[i];
        }
    }

    public virtual void Selected() { }

    public virtual void Deselected() { }

    public abstract void Use(BattlePlayer player, List<BaseBattleCharacter> targets);

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


