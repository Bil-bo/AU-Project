
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EnemySpawn : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }

    private EnemySpawnerInfo SpawnInfo = new()
    {
        Clear = false,
    };

    public GameObject Trigger(string floorID, int objectID)
    {
        ID = floorID+"Object"+objectID;


        if (PlayerPrefs.HasKey(ID))
        {

            SpawnInfo = JsonUtility.FromJson<EnemySpawnerInfo>(PlayerPrefs.GetString(ID));

            if (SpawnInfo.Clear)
            {

                gameObject.SetActive(false);
                return null;
            }

            else if (GameData.Instance.EnemySpawnerID == ID)
            {
                SpawnInfo.Clear = true;
                PlayerPrefs.SetString(ID, JsonUtility.ToJson(SpawnInfo));
                gameObject.SetActive(false);
                return null;
            }

            else
            {
                Addressables.LoadAssetAsync<GameObject>(SpawnInfo.EnemyID).Completed += (result) =>
                {

                    GameObject Enemy = Instantiate(result.Result, transform.position, Quaternion.identity);
                    Enemy.transform.SetParent(transform.parent, true);
                    EnemyPropsRoaming enemyData = Enemy.GetComponent<EnemyPropsRoaming>();
                    enemyData.SpawnerID = ID;
                    SpawnInfo.EnemiesToPassID.ForEach(enemy => Addressables.LoadAssetAsync<GameObject>(enemy).Completed += (result) =>
                    {
                        enemyData.battleEnemyInfos.Add(result.Result);
                    });

                    SpawnInfo.CardStore.ForEach(card => Addressables.LoadAssetAsync<GameObject>(card).Completed += (result) =>
                    {
                        enemyData.CardRewardsInfos.Add(result.Result);
                    });
                };

                return gameObject;

            }
        }

        else
        {
            SpawnInfo.Clear = false;
            StartCoroutine(AddressablesManager.Instance.GetRandomItem(AddressType.OVERWORLD_ENEMY, result =>
            {
                SpawnInfo.EnemyID = result.Key;
                GameObject Enemy = Instantiate(result.Value, transform.position, Quaternion.identity);

                Enemy.transform.SetParent(transform.parent, true);    
                EnemyPropsRoaming enemyData = Enemy.GetComponent<EnemyPropsRoaming>();
                enemyData.SpawnerID = ID;

                int EnemiesAmount = UnityEngine.Random.Range(1, 5);
                int CardsAmount = UnityEngine.Random.Range(1, 4);

                StartCoroutine(AddressablesManager.Instance.GetRandomItems(EnemiesAmount, AddressType.BATTLE_ENEMY, result =>
                {
                    result.ForEach(item => 
                    {
                        SpawnInfo.EnemiesToPassID.Add(item.Key);
                        enemyData.battleEnemyInfos.Add(item.Value);
                    });
                    PlayerPrefs.SetString(ID, JsonUtility.ToJson(SpawnInfo));

                }));

                StartCoroutine(AddressablesManager.Instance.GetRandomItems(CardsAmount, AddressType.CARD, result =>
                {
                    result.ForEach(item =>
                    {
                        SpawnInfo.CardStore.Add(item.Key);
                        enemyData.CardRewardsInfos.Add(item.Value);
                    });
                    PlayerPrefs.SetString(ID, JsonUtility.ToJson(SpawnInfo));

                }));


            }));


            return gameObject;
        }
    }
}

[Serializable]
public class EnemySpawnerInfo : TreasureChestInfo
{
    public string EnemyID;
    public List<string> EnemiesToPassID = new();

}
