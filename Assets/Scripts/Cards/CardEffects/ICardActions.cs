using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// Abstract class that all card effects inherit from
// Heavily Inspired by this post https://discussions.unity.com/t/what-is-the-most-effective-way-to-structure-card-effects-in-a-single-player-game/216011/2
public interface ICardActions
{
    public void Effect();

}

