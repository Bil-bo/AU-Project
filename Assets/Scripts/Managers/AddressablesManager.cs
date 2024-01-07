using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AddressablesManager : MonoBehaviour
{
    private List<string> CardAddresses = new List<string>();
    private List<string> FloorPlanAddresses = new List<string>();
    private List<string> OverWorldEnemyAddresses = new List<string>();
    private List<string> BattleEnemyAddresses = new List<string>();

    public event Action ListsReady;


    public static AddressablesManager Instance;

    private void Awake()
    {
        if ( Instance == null)
        {
            Instance = this;
        }

        else Destroy( gameObject );  
    }

    public IEnumerator GenerateLists()
    {
        var cardHandler = Addressables.LoadResourceLocationsAsync("Card");
        yield return cardHandler.Task;

        OnGenerated(cardHandler, CardAddresses);

        var floorHandler = Addressables.LoadResourceLocationsAsync("FloorPlan");
        yield return floorHandler.Task;

        OnGenerated(floorHandler, FloorPlanAddresses);   
        
        var OverWorldEnemyHandler = Addressables.LoadResourceLocationsAsync("OverWorld");
        yield return OverWorldEnemyHandler.Task;

        OnGenerated(OverWorldEnemyHandler, OverWorldEnemyAddresses);    
        
        var BattleEnemyHandler = Addressables.LoadResourceLocationsAsync("Battle");
        yield return BattleEnemyHandler.Task;

        OnGenerated(BattleEnemyHandler, BattleEnemyAddresses);

        ListsReady?.Invoke();

        
    }

    private void OnGenerated(AsyncOperationHandle<IList<IResourceLocation>> handler, List<string> addresses)
    {
        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var resourceLocation in handler.Result)
            {
                Debug.Log(resourceLocation);
                addresses.Add(resourceLocation.PrimaryKey);
                Debug.Log(addresses.Count);
            }
        }
    }


    public IEnumerator GetFloors (List<Vector2Int> coordinates, int level, Action<Dictionary<Vector2Int, GameObject>> result) 
    {
        Dictionary<Vector2Int, GameObject>floors = new Dictionary<Vector2Int, GameObject>();

        yield return StartCoroutine(GenerateRandomFloors(coordinates, level, result => floors = result));

        result?.Invoke(floors);


    }


    private IEnumerator GenerateRandomFloors(List<Vector2Int> coordinates, int level, Action<Dictionary<Vector2Int, GameObject>> result)
    {
        Dictionary<Vector2Int,GameObject> coordToFloor = new ();
        foreach (var coordinate in coordinates)
        {
            int RandId = UnityEngine.Random.Range(0, Mathf.Max(1, FloorPlanAddresses.Count));
            var handle = Addressables.LoadAssetAsync<GameObject>(FloorPlanAddresses[RandId]);
            yield return handle;
            PlayerPrefs.SetString("Level" + level + coordinate, FloorPlanAddresses[RandId]);

            coordToFloor[coordinate] = handle.Result;
        }

        result?.Invoke(coordToFloor);
    }


    public IEnumerator GetRandomItems(int amount, AddressType PullList, Action<List<KeyValuePair<string, GameObject>>> result)
    {
        List<string> addresses;
        List<KeyValuePair<string, GameObject>> items = new List<KeyValuePair<string, GameObject>>();

        switch (PullList)
        {
            case AddressType.CARD:
                addresses = CardAddresses; 
                break;            
            case AddressType.FLOORPLAN:
                addresses = FloorPlanAddresses; 
                break;            
            case AddressType.OVERWORLD_ENEMY:
                addresses = OverWorldEnemyAddresses; 
                break;            
            case AddressType.BATTLE_ENEMY:
                addresses = BattleEnemyAddresses; 
                break;
            default:
                Debug.LogError("Used an enum not in list");
                addresses = new List<string>();
                break;

        }


        yield return StartCoroutine(GenerateRandomItems(amount, addresses, result => items = result));

        result.Invoke(items);
    }


    private IEnumerator GenerateRandomItems(int amount, List<string> addresses, Action<List<KeyValuePair<string, GameObject>>> result)
    {
        List<KeyValuePair<string, GameObject>> items = new List<KeyValuePair<string, GameObject>>();
        for (int i = 0; i < amount; i++)
        {
            int RandId = UnityEngine.Random.Range(0, addresses.Count);
            var handle = Addressables.LoadAssetAsync<GameObject>(addresses[RandId]);
            yield return handle;
            items.Add(new KeyValuePair<string, GameObject>(addresses[RandId], handle.Result));
        }

        result?.Invoke(items);
    }

    public IEnumerator GetRandomItem(AddressType PullList, Action<KeyValuePair<string, GameObject>> result)
    {
        List<string> addresses;
        KeyValuePair<string, GameObject> item = new KeyValuePair<string, GameObject>();

        switch (PullList)
        {
            case AddressType.CARD:
                addresses = CardAddresses;
                break;
            case AddressType.FLOORPLAN:
                addresses = FloorPlanAddresses;
                break;
            case AddressType.OVERWORLD_ENEMY:
                addresses = OverWorldEnemyAddresses;
                break;
            case AddressType.BATTLE_ENEMY:
                addresses = BattleEnemyAddresses;
                break;
            default:
                Debug.LogError("Used an enum not in list");
                addresses = new List<string>();
                break;

        }


        yield return StartCoroutine(GenerateRandomItem(addresses, result => item = result));

        result.Invoke(item);
    }


    private IEnumerator GenerateRandomItem(List<string> addresses, Action<KeyValuePair<string, GameObject>> result)
    {
        KeyValuePair<string, GameObject> item;

        int RandId = UnityEngine.Random.Range(0, addresses.Count);
        var handle = Addressables.LoadAssetAsync<GameObject>(addresses[RandId]);
        yield return handle;

        item  = new KeyValuePair<string, GameObject>(addresses[RandId], handle.Result);


        result?.Invoke(item);
    }

}

public enum AddressType
{
    CARD,
    FLOORPLAN,
    OVERWORLD_ENEMY,
    BATTLE_ENEMY
    
}