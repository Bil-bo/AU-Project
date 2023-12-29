using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEditor;
using System.Runtime.CompilerServices;

public class BattlePlayer : BaseBattleCharacter
{

    public bool isMyTurn = false;
    public int maxHand { get; set; } = 3;

    private BattleInfo _info;
    public BattleInfo info
    {
        get {  return _info; }
        set 
        {
            _info = value;
            maxHealth = _info.maxHealth;
            Name = _info.Name;
            MaxEnergy = _info.MaxEnergy;
        }
    }

    public int MaxEnergy { get; set; }

    private int _CurrentEnergy;

    public int CurrentEnergy 
    {
        get { return _CurrentEnergy; }
        set 
        {
            _CurrentEnergy = Mathf.Clamp(value, 0, 999);
            if (_CurrentEnergy <= 0 )
            {
                FinishTurn();
            }
        }
    }


   



    // Awake called to avoid race conditions with the coroutine
    private void Awake()
    {
        hudManager = GameObject.Find("Canvas").GetComponent<HUDManager>();
        attack = 0;
        defense = 0;
    }


    // TODO: Find a way to remove this
    public override void Start()
    {

        base.Start();

    }


    public override IEnumerator DoTurn()
    {

        ProcessStatusEffects(); //Both
        isMyTurn = true;
        CurrentEnergy = MaxEnergy;
        
        hudManager.UpdateTurnText(Name);
        UpdateHealthBar();

        while (isMyTurn)
        {
            yield return null;
        }

        UpdateHealthBar();
    }
    
    public void FinishTurn()
    {
        isMyTurn = false;
    }

}
