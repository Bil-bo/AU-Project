

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Here, we will hold enemy info in general, this will be different for each kind of enemy

[CreateAssetMenu(fileName = "New EnemyInfo", menuName = "EnemyInfo")] //Creating a new asset
public class BattleEnemyInfo : ScriptableObject
{
    public int maxHP;
    public string enemyName;

    public int atk;
}
