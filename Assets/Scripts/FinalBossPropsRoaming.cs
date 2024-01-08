using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FinalBossPropsRoaming : EnemyPropsRoaming, IFloorObject
{
    public string ID { get; set; }
    public EnemySpawnerInfo SpawnInfo { get; set; } = new EnemySpawnerInfo();

    public GameObject Trigger(string floorID, int ObjectID)
    {
        ID = floorID + "Object" + ObjectID;

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

                SpawnerID = ID;
                SpawnInfo.EnemiesToPassID.ForEach(enemy => Addressables.LoadAssetAsync<GameObject>(enemy).Completed += (result) =>
                {
                    battleEnemyInfos.Add(result.Result);
                });

                SpawnInfo.CardStore.ForEach(card => Addressables.LoadAssetAsync<GameObject>(card).Completed += (result) =>
                {
                    CardRewardsInfos.Add(result.Result);
                });


                return gameObject;

            }
        }

        else
        {

            SpawnInfo.Clear = false;

            int EnemiesAmount = UnityEngine.Random.Range(1, 3);
            int CardsAmount = UnityEngine.Random.Range(1, 3);

            StartCoroutine(AddressablesManager.Instance.GetRandomItems(EnemiesAmount, AddressType.BATTLE_ENEMY, result =>
            {
                result.ForEach(item =>
                {
                    SpawnInfo.EnemiesToPassID.Add(item.Key);
                    battleEnemyInfos.Add(item.Value);
                });
                PlayerPrefs.SetString(ID, JsonUtility.ToJson(SpawnInfo));

            }));

            StartCoroutine(AddressablesManager.Instance.GetRandomItems(CardsAmount, AddressType.CARD, result =>
            {
                result.ForEach(item =>
                {
                    SpawnInfo.CardStore.Add(item.Key);
                    CardRewardsInfos.Add(item.Value);
                });
                PlayerPrefs.SetString(ID, JsonUtility.ToJson(SpawnInfo));

            }));

            return gameObject;
        }
    }
}
