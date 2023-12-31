using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;


// Treasure chest, for roaming rewards
public class TreasureChest : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }

    public TreasureChestInfo ChestInfo = new();
    
    public List<GameObject> ChestList = new List<GameObject>();


    public GameObject Trigger(string floorID, int objectID)
    {
        ID = floorID + "Object" + objectID;



        if (PlayerPrefs.HasKey(ID))
        {
            Debug.Log("Key In PlayerPrefs");
            ChestInfo = JsonUtility.FromJson<TreasureChestInfo>(PlayerPrefs.GetString(ID));

            if (ChestInfo.Clear)
            {
                Debug.Log("Area Has Been Cleared");
                gameObject.SetActive(false);
            }

            else
            {
                Debug.Log("Area Has Been Cleared");
                ChestInfo.CardStore.ForEach(card => Addressables.LoadAssetAsync<GameObject>(card).Completed += (result) =>
                {
                    ChestList.Add(result.Result);
                    
                });

            }
        }

        else
        {
            Debug.Log("Spawning New Items");
            ChestInfo.Clear = false;
            int CardsAmount = UnityEngine.Random.Range(1, 4);
            StartCoroutine(AddressablesManager.Instance.GetRandomItems(CardsAmount, AddressType.CARD, result =>
            {
                result.ForEach(item =>
                {
                    ChestInfo.CardStore.Add(item.Key);
                    ChestList.Add(item.Value);
                });
                PlayerPrefs.SetString(ID, JsonUtility.ToJson(ChestInfo));

            }));
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerRoaming"))
        {
            gameObject.SetActive(false);
            ChestInfo.Clear = true;
            PlayerPrefs.SetString(ID, JsonUtility.ToJson(ChestInfo));

            TreasureCollectedEvent gameEvent = new TreasureCollectedEvent()
            {
                Collider = other.GetComponent<PlayerPropsRoaming>(),
                Treasure = ChestList
            };

            EventManager.Broadcast(gameEvent);

        }
    }


}

public class SaveInfo 
{
    public bool Clear;
}

public class TreasureChestInfo : SaveInfo
{
    public List<string> CardStore = new();

}
