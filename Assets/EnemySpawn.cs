using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EnemySpawn : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }

    private EnemySpawnerInfo SpawnInfo = new();

    public GameObject Trigger(string floorID, int objectID)
    {
        ID = floorID+gameObject.name+objectID;
        Debug.Log(ID);



        if (PlayerPrefs.HasKey(ID))
        {
            Debug.Log("Key In PlayerPrefs");
            SpawnInfo = JsonUtility.FromJson<EnemySpawnerInfo>(PlayerPrefs.GetString(ID));

            if (SpawnInfo.Clear)
            {
                Debug.Log("Area Has Been Cleared");
                gameObject.SetActive(false);
                return null;
            }

            else if (GameData.Instance.EnemySpawnerID == ID)
            {
                Debug.Log("Battle Has Been Won");
                SpawnInfo.Clear = true;
                PlayerPrefs.SetString(ID, JsonUtility.ToJson(SpawnInfo));
                gameObject.SetActive(false);
                return null;
            }

            else
            {
                Addressables.LoadAssetAsync<GameObject>(SpawnInfo.EnemyID).Completed += (result) =>
                {
                    Debug.Log("Respawning Items");
                    GameObject Enemy = Instantiate(result.Result, transform.position, Quaternion.identity);
                    Enemy.transform.SetParent(transform.parent, true);
                    EnemyPropsRoaming enemyData = Enemy.GetComponent<EnemyPropsRoaming>();
                    enemyData.SpawnerID = ID;
                    SpawnInfo.EnemiesToPassID.ForEach(enemy => Addressables.LoadAssetAsync<GameObject>(enemy).Completed += (result) =>
                    {
                        enemyData.battleEnemyInfos.Add(result.Result);
                    });

                    SpawnInfo.CardsToPassID.ForEach(card => Addressables.LoadAssetAsync<GameObject>(card).Completed += (result) =>
                    {
                        enemyData.CardRewardsInfos.Add(result.Result);
                    });
                };

                return gameObject;

            }
        }

        else
        {
            Debug.Log("Spawning New Items");
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
                        SpawnInfo.CardsToPassID.Add(item.Key);
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
public class EnemySpawnerInfo
{
    public bool Clear;
    public string EnemyID;

    public List<string> CardsToPassID = new();
    public List<string> EnemiesToPassID = new();

}
