using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RescueSpawn : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }
    private RescueInfo Rescue = new();

    public GameObject Trigger(string floorID, int objectID)
    {
        ID = floorID + "Object" + objectID;



        if (PlayerPrefs.HasKey(ID))
        {

            Rescue = JsonUtility.FromJson<RescueInfo>(PlayerPrefs.GetString(ID));

            if (Rescue.Clear)
            {

                gameObject.SetActive(false);
            }

            else if (GameData.Instance.EnemySpawnerID == ID)
            {

                Rescue.Clear = true;
                PlayerPrefs.SetString(ID, JsonUtility.ToJson(Rescue));
            }

            else
            {
                Addressables.LoadAssetAsync<GameObject>(Rescue.RescueID).Completed += (result) =>
                {
                    GameObject player = Instantiate(result.Result, transform.position, Quaternion.identity);
                    player.transform.SetParent(transform.parent, true);
                    RescuePropsBound playerData = player.GetComponent<RescuePropsBound>();
                    playerData.SpawnerID = ID;  

                };


            }
        }

        else
        {

            Rescue.Clear = false;
            StartCoroutine(AddressablesManager.Instance.GetRandomItem(AddressType.OVERWORLD_ENEMY, result =>
            {
                Rescue.RescueID = result.Key;
                GameObject player = Instantiate(result.Value, transform.position, Quaternion.identity);

                player.transform.SetParent(transform.parent, true);
            }));
        }
        return null;

    }
}

public class RescueInfo : SaveInfo
{
    public string RescueID;
    public BattleInfo Info;

}
