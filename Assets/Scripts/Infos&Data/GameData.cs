using System.Collections.Generic;
using UnityEngine;


// For data Persistance Between Scenes
public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public GameObject PlayerPrefab;

    public List<GameObject> battleEnemies;

    public List<GameObject> CardRewards;

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


    // Adding a new player to the party
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
        EnemySpawnerID = null; 
        CardRewards = new List<GameObject>();
        BattlePlayers = new Dictionary<GameObject, List<GameObject>>();

        foreach(Transform c in transform)
        {
            Destroy(c.gameObject);
        }
        transform.DetachChildren();

    }
}