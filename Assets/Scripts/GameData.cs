using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


// For data Persistance Between Scenes
public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public GameObject PlayerPrefab;

    public List<GameObject> battleEnemies;
    public bool isPuzzleComplete { get; set; }

    public Dictionary<GameObject, List<GameObject>> BattlePlayers = new();

    public string EnemySpawnerID;


    public CardFactory Factory;






    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPlayer(BattleInfo playerInfo)
    {
        GameObject newPlayer = Instantiate(PlayerPrefab, this.transform);
        newPlayer.GetComponent<BattlePlayer>().info = playerInfo;
        List<GameObject> Cards = CardFactory.CreateCards(playerInfo.Deck, this.transform, newPlayer.GetComponent<BattlePlayer>().CharID);
        BattlePlayers.Add(newPlayer, Cards);
        newPlayer.SetActive(false);

    }

    public void Restart()
    {
        isPuzzleComplete = false;
    }
}