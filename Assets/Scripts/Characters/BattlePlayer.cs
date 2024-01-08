using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEditor;
using System;


// Base class for all playable characters
// Since all character share the same behaviour, you only need a different model for cool effects
public class BattlePlayer : BaseBattleCharacter
{
    // for coroutine loop
    public bool isMyTurn = false;

    // Simple Action for observer
    public Action<int, int> OnEnergyChanged;


    private BattleInfo _info;
    public BattleInfo info
    {
        get {  return _info; }
        set 
        {
            _info = value;
            maxHealth = _info.maxHealth;
            CurrentHealth = _info.maxHealth;
            Name = _info.Name;
            MaxEnergy = _info.MaxEnergy;
            MaxHand = _info.MaxHandSize;
        }
    }

    public int MaxHand { get; set; } = 3;

    private int _MaxEnergy;

    public int MaxEnergy
    {
        get { return _MaxEnergy; }
        set
        {
            _MaxEnergy = Mathf.Clamp(value, 1, 999);
            OnEnergyChanged?.Invoke(CurrentEnergy, MaxEnergy);
        }
    }

    private int _CurrentEnergy;

    public int CurrentEnergy 
    {
        get { return _CurrentEnergy; }
        set 
        {
            _CurrentEnergy = Mathf.Max(value, 0);
            OnEnergyChanged?.Invoke(CurrentEnergy, MaxEnergy);
        }
    }

    // Lock a player's turn in until otherwise
    public override IEnumerator DoTurn()
    {
        isMyTurn = true;
        CurrentEnergy = MaxEnergy;
        

        while (isMyTurn)
        {
            if ((PlayerPrefs.GetInt("AutoEndTurn") == 1) && CurrentEnergy <= 0)
            {
                isMyTurn = false;
            }
            yield return null;
        }

    }
    

    // For the deck buttons
    public void FinishTurn()
    {
        isMyTurn = false;
    }

    // For calling player death
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (CurrentHealth == 0)
        {
            isMyTurn = false;
            PlayerDeathEvent playerDied = new PlayerDeathEvent()
            {
                ID = CharID,
                player = this
            };

            EventManager.Broadcast(playerDied);
        }
    }

}
