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

    private Dictionary<string, bool> doors = new Dictionary<string, bool>();

    public GameObject [] battleEnemies;
    public bool isPuzzleComplete { get; set; }

    public Dictionary<GameObject, List<GameObject>> BattlePlayers = new();

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
        List<GameObject> Cards = CardFactory.CreateCards(playerInfo.Deck, this.transform);
        Debug.Log(newPlayer.ToString());
        Debug.Log(Cards.ToString());
        BattlePlayers.Add(newPlayer, Cards);
        newPlayer.SetActive(false);
      
    }
    public void AddDoor(Door door)
    {
        doors.Add(door.ID, door.isOpen);    
    }

    public void LoadDoor(Door door)
    {
        if (doors.ContainsKey(door.ID))
        {
            door.isOpen = doors[door.ID];
        }
    }

    public void SetDoor(Door door, bool newState)
    {
        if (doors.ContainsKey(door.ID)) 
        {
            doors[door.ID] = newState;
        }
    }

    public bool FindDoor(Door door)
    {
        return doors.ContainsKey(door.ID);
    }

    public void Restart()
    {
        isPuzzleComplete = false;
        doors.Clear();
    }
}