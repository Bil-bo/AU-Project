using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// For data Persistance Between Scenes
public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public GameObject PlayerPrefab;
    public GameObject CardPrefab;

    private Dictionary<string, bool> doors = new Dictionary<string, bool>();

    public GameObject [] battleEnemies;
    public bool isPuzzleComplete { get; set; }

    public Dictionary<GameObject, List<GameObject>> BattlePlayers = new();






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
        List<GameObject> Cards = AddCards(playerInfo.Deck);
        Debug.Log(newPlayer.ToString());
        Debug.Log(Cards.ToString());
        BattlePlayers.Add(newPlayer, Cards);
        newPlayer.SetActive(false);
      
    }

    private List<GameObject> AddCards (List<GameObject> initDeck)
    {
        List<GameObject> cards = new List<GameObject>();    
        foreach (GameObject card in initDeck)
        {
            GameObject newCard = Instantiate(CardPrefab, this.transform);
            Instantiate(card, newCard.transform);
            newCard.SetActive(false);
            cards.Add(newCard);
        }
        return cards;
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