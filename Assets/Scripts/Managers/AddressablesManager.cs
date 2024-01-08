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
    private List<string> OverWorldBossEnemyAddresses = new List<string>();
    private List<string> BattleBossEnemyAddresses = new List<string>();
    private List<string> RescuePlayers = new List<string>();

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

        var OverWorldBossEnemyHandler = Addressables.LoadResourceLocationsAsync("RoamingBoss");
        yield return OverWorldBossEnemyHandler.Task;

        OnGenerated(OverWorldBossEnemyHandler, OverWorldBossEnemyAddresses);

        var BossBattleEnemyHandler = Addressables.LoadResourceLocationsAsync("Boss");
        yield return BossBattleEnemyHandler.Task;

        OnGenerated(BossBattleEnemyHandler, BattleBossEnemyAddresses);
                
        var RescueHandler = Addressables.LoadResourceLocationsAsync("Support");
        yield return RescueHandler.Task;

        OnGenerated(RescueHandler, RescuePlayers);

        ListsReady?.Invoke();

    }

    private void OnGenerated(AsyncOperationHandle<IList<IResourceLocation>> handler, List<string> addresses)
    {
        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (IResourceLocation resourceLocation in handler.Result)
            {
                addresses.Add(resourceLocation.PrimaryKey);
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

    private List<string> GetAddressList(AddressType pull)
    {
        switch (pull)
        {
            case AddressType.CARD:
                return CardAddresses;

            case AddressType.FLOORPLAN:
                return FloorPlanAddresses;

            case AddressType.OVERWORLD_ENEMY:
                return OverWorldEnemyAddresses;

            case AddressType.BATTLE_ENEMY:
                return BattleEnemyAddresses;

            case AddressType.BOSS:
                return BattleBossEnemyAddresses;

            case AddressType.OVERWORLD_BOSS:
                return OverWorldBossEnemyAddresses;

            case AddressType.SUPPORT:
                return RescuePlayers;

            default:
                Debug.LogError("Used an enum not in list");
                throw new NotImplementedException();

        }
    }


    public IEnumerator GetRandomItems(int amount, AddressType PullList, Action<List<KeyValuePair<string, GameObject>>> result)
    {
        List<string> addresses = GetAddressList(PullList);
        List<KeyValuePair<string, GameObject>> items = new List<KeyValuePair<string, GameObject>>();
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
        List<string> addresses = GetAddressList(PullList);
        KeyValuePair<string, GameObject> item = new KeyValuePair<string, GameObject>();
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
    BATTLE_ENEMY,
    OVERWORLD_BOSS,
    BOSS,
    SUPPORT
    
}