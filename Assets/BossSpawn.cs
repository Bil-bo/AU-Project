using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEditor.Progress;

public class BossSpawn : MonoBehaviour, IFloorObject
{
    public string ID { get;  set; }

    private EnemySpawnerInfo BossSpawnInfo = new();

    public GameObject Trigger(string floorID, int objectID)
    {
        ID = floorID + gameObject.name + objectID;
        if (PlayerPrefs.HasKey(ID))
        {

            BossSpawnInfo = JsonUtility.FromJson<EnemySpawnerInfo>(PlayerPrefs.GetString(ID));

            if (BossSpawnInfo.Clear)
            {

                gameObject.SetActive(false);
                return null;
            }

            else if (GameData.Instance.EnemySpawnerID == ID)
            {

                BossSpawnInfo.Clear = true;
                PlayerPrefs.SetString(ID, JsonUtility.ToJson(BossSpawnInfo));
                
                BossDefeatedEvent defeatedEvent = new BossDefeatedEvent();
                EventManager.Broadcast(defeatedEvent);
                transform.parent.transform.Find("Ladder").gameObject.SetActive(true);
                gameObject.SetActive(false);
                return null;
            }

            else
            {
                Addressables.LoadAssetAsync<GameObject>(BossSpawnInfo.EnemyID).Completed += (result) =>
                {

                    GameObject Enemy = Instantiate(result.Result, transform.position, Quaternion.identity);
                    Enemy.transform.SetParent(transform.parent, true);
                    EnemyPropsRoaming enemyData = Enemy.GetComponent<EnemyPropsRoaming>();
                    enemyData.SpawnerID = ID;
                    BossSpawnInfo.EnemiesToPassID.ForEach(enemy => Addressables.LoadAssetAsync<GameObject>(enemy).Completed += (result) =>
                    {
                        enemyData.battleEnemyInfos.Add(result.Result);
                    });

                    BossSpawnInfo.CardStore.ForEach(card => Addressables.LoadAssetAsync<GameObject>(card).Completed += (result) =>
                    {
                        enemyData.CardRewardsInfos.Add(result.Result);
                    });
                };

                return gameObject;

            }
        }

        else
        {

            BossSpawnInfo.Clear = false;
            StartCoroutine(AddressablesManager.Instance.GetRandomItem(AddressType.OVERWORLD_BOSS, result =>
            {
                BossSpawnInfo.EnemyID = result.Key;
                GameObject Enemy = Instantiate(result.Value, transform.position, Quaternion.identity);

                Enemy.transform.SetParent(transform.parent, true);
                EnemyPropsRoaming enemyData = Enemy.GetComponent<EnemyPropsRoaming>();
                enemyData.SpawnerID = ID;

                int EnemiesAmount = UnityEngine.Random.Range(1, 2);
                int CardsAmount = 3;

                StartCoroutine(AddressablesManager.Instance.GetRandomItems(EnemiesAmount, AddressType.BATTLE_ENEMY, result =>
                {
                    result.ForEach(item =>
                    {
                        BossSpawnInfo.EnemiesToPassID.Add(item.Key);
                        enemyData.battleEnemyInfos.Add(item.Value);
                    });

                    PlayerPrefs.SetString(ID, JsonUtility.ToJson(BossSpawnInfo));

                }));

                StartCoroutine(AddressablesManager.Instance.GetRandomItems(CardsAmount, AddressType.CARD, result =>
                {
                    result.ForEach(item =>
                    {
                        BossSpawnInfo.CardStore.Add(item.Key);
                        enemyData.CardRewardsInfos.Add(item.Value);
                    });
                    PlayerPrefs.SetString(ID, JsonUtility.ToJson(BossSpawnInfo));

                }));

                StartCoroutine(AddressablesManager.Instance.GetRandomItem(AddressType.BOSS, result =>
                {
                    BossSpawnInfo.EnemiesToPassID.Add(result.Key);
                    enemyData.battleEnemyInfos.Add(result.Value);
                    PlayerPrefs.SetString(ID, JsonUtility.ToJson(BossSpawnInfo));

                }));



            }));


            return gameObject;
        }
    }
}
