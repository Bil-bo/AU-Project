using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// For Roaming Enemies for holding data to be passed to the battle scene

public class EnemyPropsRoaming : MonoBehaviour
{
    public List<GameObject> battleEnemyInfos;
    public List<GameObject> CardRewardsInfos;
    public string SpawnerID { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerRoaming"))
        {
            GameData.Instance.battleEnemies = battleEnemyInfos;
            GameData.Instance.CardRewards = CardRewardsInfos;
            GameData.Instance.EnemySpawnerID = SpawnerID;
            EventManager.Broadcast(new BattleTriggerEvent());
        }
    }

}
