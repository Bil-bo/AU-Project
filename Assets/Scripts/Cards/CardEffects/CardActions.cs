using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CardActions : MonoBehaviour
{

    public string Description;
    public virtual void Effect(BaseBattleCharacter target, BattlePlayer user) 
    {

    }

    public virtual string GetDescription() { return Description; } 
    public virtual string GetDescription(BattlePlayer player) { return Description; }
}
