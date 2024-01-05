using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialRoomManager : MonoBehaviour
{

    [SerializeField]
    private GameObject RescueRoom;

    [SerializeField]
    private GameObject BossRoom;

    [SerializeField]
    private GameObject Smithy;

    [SerializeField] 
    private GameObject PickUpRoom;



    [SerializeField]
    private GameObject Shop;

    [SerializeField]
    private GameObject Treasure;

    [SerializeField]
    private int RescueChance = 75;

    [SerializeField]
    private int SmithyChance = 50;

    [SerializeField]
    private int ShopChance = 60;


    public Dictionary<Vector2Int, GameObject> SpecialRoomRoll(int level, Dictionary<Vector2Int, GameObject> coordinateData)
    {
        List<Vector2Int> coordinates = coordinateData.Keys.ToList();
        if(UnityEngine.Random.Range(0, 101) < RescueChance) 
        {
            coordinateData[InsertSpecialRoom(coordinates)] = RescueRoom;
            RescueChance -= (RescueChance < 50) ? RescueChance : 10;

        }

        if (UnityEngine.Random.Range(0, 101) < ShopChance)
        {
            coordinateData[InsertSpecialRoom(coordinates)] = Shop;
        }

        if (UnityEngine.Random.Range(0, 101) < SmithyChance)
        {
            coordinateData[InsertSpecialRoom(coordinates)] = Smithy;
        }

        coordinateData[InsertSpecialRoom(coordinates)] = Treasure;

        for (int i = 0; i < 2; i++)
        {
            coordinateData[InsertSpecialRoom(coordinates)] = PickUpRoom;
        }

        coordinateData[InsertBossRoom(coordinates)] = BossRoom;



        return coordinateData;

    }

    public Vector2Int InsertSpecialRoom(List<Vector2Int> coordinates)
    {

        return WalkerFactory.CreateSpecialWalker().Walk(coordinates)[0];

    }

    public Vector2Int InsertBossRoom(List<Vector2Int> coordinates)
    {

        return WalkerFactory.CreateBossWalker().Walk(coordinates)[0];

    }



}
