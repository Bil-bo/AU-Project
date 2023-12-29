using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Card : MonoBehaviour
{
    private Transform CardBase;


    [SerializeField]
    private CardInfo _cardInfo;

    public GameObject CardView { get; set; }

    public Guid CardID { get; } = Guid.NewGuid();

    private string _Name;

    public string Name { get { return _Name; } set { _Name = value; } }

    private string Description;

    public int Cost { get; set; }

    public Dictionary<string, GameObject> Merges;

    private CardType Type;

    public Target Target { get; set; }

    public int Range { get; set; }

    private List<int> _Damage;


    public List<int> Damage { get; set; }

    public int DamageModifier { get; set; }

    private List<int> FlatDamage;

    private bool Activated = false;

    private void OnEnable()
    {
        if (!Activated)
        {
            Activated = true;
            CardView = gameObject;
            CardBase = gameObject.transform.parent;
            Name = _cardInfo.Name;
            Description = _cardInfo.Description;
            Cost = _cardInfo.Cost;
            FormMerges(_cardInfo.CardInput, _cardInfo.CardOutput);
            Type = _cardInfo.Type;
            Target = _cardInfo.Target;
            Range = _cardInfo.Range;
            Damage = _cardInfo.Damage;
            FlatDamage = _cardInfo.FlatDamage;
        }
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


}


