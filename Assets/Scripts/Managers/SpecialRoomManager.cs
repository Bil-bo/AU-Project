using System;
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
    private GameObject SuperBossRoom;

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


    public Dictionary<Vector2Int, GameObject> SpecialRoomRoll(int levelNum, int level, Dictionary<Vector2Int, GameObject> coordinateData)
    {
        List<Vector2Int> coordinates = coordinateData.Keys.ToList();
        if(UnityEngine.Random.Range(0, 101) < RescueChance) 
        {
            Vector2Int Inject = InsertSpecialRoom(coordinates);
            coordinateData[Inject] = RescueRoom;
            PlayerPrefs.SetString("Level" + level + Inject, RescueRoom.name);
            RescueChance -= (RescueChance < 50) ? RescueChance : 10;

        }

        if (UnityEngine.Random.Range(0, 101) < ShopChance)
        {
            Vector2Int Inject = InsertSpecialRoom(coordinates);
            coordinateData[Inject] = Shop;
            PlayerPrefs.SetString("Level" + level + Inject, Shop.name);
        }

        if (UnityEngine.Random.Range(0, 101) < SmithyChance)
        {
            Vector2Int Inject = InsertSpecialRoom(coordinates);
            coordinateData[Inject] = Smithy;
            PlayerPrefs.SetString("Level" + level + Inject, Smithy.name);
        }

        Vector2Int injectTreasure = InsertSpecialRoom(coordinates);
        coordinateData[injectTreasure] = Treasure;
        PlayerPrefs.SetString("Level" + level + injectTreasure, Treasure.name);

        for (int i = 0; i < 2; i++)
        {
            Vector2Int injectPickup = InsertSpecialRoom(coordinates);
            coordinateData[injectPickup] = PickUpRoom;
            PlayerPrefs.SetString("Level" + level + injectPickup, PickUpRoom.name);
        }

        Vector2Int injectBoss = InsertBossRoom(coordinates);

        if (levelNum == level+1)
        {
            coordinateData[injectBoss] = SuperBossRoom;
            PlayerPrefs.SetString("Level" + level + injectBoss, SuperBossRoom.name);
        }
        else
        {
            coordinateData[injectBoss] = BossRoom;
            PlayerPrefs.SetString("Level" + level + injectBoss, BossRoom.name);
        }




        return coordinateData;

    }

    public Vector2Int InsertSpecialRoom(List<Vector2Int> coordinates)
    {

        return WalkerFactory.CreateSpecialWalker().Walk(coordinates)[0];

    }

    public Vector2Int InsertBossRoom(List<Vector2Int> coordinates)
    {

        return WalkerFactory.CreateBossWalker(coordinates[0]).Walk(coordinates)[0];

    }



}
