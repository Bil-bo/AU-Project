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
            _CurrentEnergy = Mathf.Max(value, 0);
        }
    }


    public override IEnumerator DoTurn()
    {
        isMyTurn = true;
        CurrentEnergy = MaxEnergy;
        

        while (isMyTurn)
        {
            yield return null;
        }

    }
    
    public void FinishTurn()
    {
        isMyTurn = false;
    }


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
