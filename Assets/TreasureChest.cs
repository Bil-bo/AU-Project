using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TreasureChest : MonoBehaviour, IFloorObject
{
    public string ID { get; set; }

    public TreasureChestInfo ChestInfo = new();
    
    public List<GameObject> ChestList = new List<GameObject>();

    public GameObject Trigger(string floorID, int objectID)
    {
        ID = floorID + gameObject.name + objectID;
        Debug.Log(ID);



        if (PlayerPrefs.HasKey(ID))
        {
            Debug.Log("Key In PlayerPrefs");
            ChestInfo = JsonUtility.FromJson<TreasureChestInfo>(PlayerPrefs.GetString(ID));

            if (ChestInfo.Clear)
            {
                Debug.Log("Area Has Been Cleared");
                gameObject.SetActive(false);
                return null;
            }

            else
            {
                ChestInfo.CardStore.ForEach(card => Addressables.LoadAssetAsync<GameObject>(card).Completed += (result) =>
                {
                    ChestList.Add(result.Result);
                    
                });
                return gameObject;

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
                PlayerPrefs.SetString(ID, JsonUtility.ToJson(ChestList));

            }));


            return gameObject;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("The Player Touched me");
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
