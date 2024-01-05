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

        ListsReady?.Invoke();

        
    }

    private void OnGenerated(AsyncOperationHandle<IList<IResourceLocation>> handler, List<string> addresses)
    {
        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var resourceLocation in handler.Result)
            {
                addresses.Add(resourceLocation.PrimaryKey);
            }
        }
    }


    public IEnumerator GetFloors (List<Vector2Int> coordinates, Action<Dictionary<Vector2Int, GameObject>> result) 
    {
        Dictionary<Vector2Int, GameObject>floors = new Dictionary<Vector2Int, GameObject>();

        yield return StartCoroutine(GenerateRandomFloors(coordinates, result => floors = result));

        result?.Invoke(floors);


    }


    private IEnumerator GenerateRandomFloors(List<Vector2Int> coordinates, Action<Dictionary<Vector2Int, GameObject>> result)
    {
        Dictionary<Vector2Int,GameObject> coordToFloor = new ();
        foreach (var coordinate in coordinates)
        {
            int RandId = UnityEngine.Random.Range(0, Mathf.Max(1, FloorPlanAddresses.Count));
            var handle = Addressables.LoadAssetAsync<GameObject>(FloorPlanAddresses[RandId]);
            yield return handle;
            coordToFloor[coordinate] = handle.Result;
        }

        result?.Invoke(coordToFloor);
    }


    public List<GameObject> GetRandomCards(int amount)
    {

        List<GameObject> cards = new List<GameObject>();

        StartCoroutine(GenerateRandomCards(amount, result => cards = result));

        return cards;
    }


    private IEnumerator GenerateRandomCards(int amount, Action<List<GameObject>> result)
    {
        int CompletedCount = 0;
        List<GameObject> cards = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            int RandId = UnityEngine.Random.Range(0, CardAddresses.Count);
            var handle = Addressables.LoadAssetAsync<GameObject>(CardAddresses[RandId]);
            yield return handle;
            cards.Add(handle.Result);
            CompletedCount++;
            if (CompletedCount == amount)
            {
                result?.Invoke(cards);
            }
        }
    }

    public GameObject GetRandomCard()
    {
        GameObject card = new();
        StartCoroutine(GenerateRandomCard(result => card = result));
        return card;

    }


    private IEnumerator GenerateRandomCard(Action<GameObject> result)
    {
        int RandId = UnityEngine.Random.Range(0, CardAddresses.Count);
        var handle = Addressables.LoadAssetAsync<GameObject>(CardAddresses[RandId]);
        yield return handle;
        result?.Invoke(handle.Result);
    }

}