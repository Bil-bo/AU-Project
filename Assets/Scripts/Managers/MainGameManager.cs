    using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;


// For managing the main scene
// Slightly Deprecated by GameData
public class MainGameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CeilingPrefab;

    [SerializeField]
    private GameObject WallPrefab;

    [SerializeField]
    private GameObject WallWithDoorPrefab;

    [SerializeField]
    private GameObject LevelStart;

    [SerializeField]
    private AddressablesManager Addresses;

    [SerializeField]
    private Transform Walls;
    
    [SerializeField]
    private Transform Floors;

    [SerializeField]
    private Transform Ceilings;

    [SerializeField] 
    private Transform Doors;

    [SerializeField]
    private SpecialRoomManager SpecialRoomManager;

    public int GridX = 10;
    public int GridY = 10;
    public int LevelAmount = 1;

    private GameObject[] Enemies;
    private GameObject[] Pickups;
    public GameObject player;
    private static bool Init = false;


    

    // Start is called before the first frame update
    void Start()
    {
        Enemies = GameObject.FindGameObjectsWithTag("Enemy").OrderBy(enemy => enemy.name).ToArray();
        Pickups = GameObject.FindGameObjectsWithTag("Pickup").OrderBy(enemy => enemy.name).ToArray();
        Init = PlayerPrefs.GetInt("Init") == 1;
        Debug.Log(Init);

        if (Init)
        {
            Destroy(Addresses.gameObject);
            // Load the enabled state for each enemy and set it
            for (int i = 0; i < Enemies.Length; i++)
            {
                int enemyEnabled = PlayerPrefs.GetInt("Enemy" + i + "Enabled");
                Enemies[i].SetActive(enemyEnabled == 1);
            }
            // Load the enabled state for each pickup and set it
            for (int i = 0; i < Pickups.Length; i++)
            {
                int pickupEnabled = PlayerPrefs.GetInt("Pickup" + i + "Enabled");
                Pickups[i].SetActive(pickupEnabled == 1);
            }
            // Load the player's position and set it
            float playerX = PlayerPrefs.GetFloat("PlayerX");
            float playerY = PlayerPrefs.GetFloat("PlayerY");
            float playerZ = PlayerPrefs.GetFloat("PlayerZ");
            player.transform.position = new Vector3(playerX, playerY, playerZ);
            
        }
        else
        {
            PlayerPrefs.SetInt("Init", 1);

            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

            StartCoroutine(Addresses.GenerateLists());
            Addresses.ListsReady += () => StartCoroutine(ConstructPlayArea());


            Init = true;
            PlayerPrefs.SetInt("PickupsCollected", 0);
            GameData.Instance.AddPlayer(player.GetComponent<PlayerPropsRoaming>().BattleInfo);
            Debug.Log("In Here");



        }
    }
     
    // For Saving data between scenes before starting the battle
    public void EnterBattle()
    {
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);


        for (int i = 0; i < Enemies.Length; i++)
        {
            PlayerPrefs.SetInt("Enemy" + i + "Enabled", Enemies[i].activeSelf ? 1 : 0);
        }
        for (int i = 0; i < Pickups.Length; i++)
        {
            PlayerPrefs.SetInt("Pickup" + i + "Enabled", Pickups[i].activeSelf ? 1 : 0);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator ConstructPlayArea() 
    {

        for (int i = 0; i < LevelAmount; i++)
        {
            List<Vector2Int> coordinates = new List<Vector2Int>();
            Dictionary<Vector2Int, GameObject> coordinateData = new Dictionary<Vector2Int, GameObject>();

            int randX = UnityEngine.Random.Range(0, GridX);
            int randY = UnityEngine.Random.Range(0, GridY);

            Vector2Int startPos = new Vector2Int(randX, randY);
            coordinates.Add(startPos);
            Debug.Log(startPos);


            coordinateData[startPos] = LevelStart;

            int RoomAmount = Mathf.RoundToInt(UnityEngine.Random.Range(0, 3) + 5 + ((i + 1) * 2.5f));

            while (coordinates.Count < RoomAmount) 
            {
                coordinates.AddRange(WalkerFactory.CreateWalker(new Vector2Int(GridX, GridY), RoomAmount - coordinates.Count).Walk(coordinates));
            }




            yield return StartCoroutine(Addresses.GetFloors(coordinates.Skip(1).ToList(),
                result => coordinateData = coordinateData.Concat(result).ToDictionary(a => a.Key, a => a.Value)));


            coordinateData = SpecialRoomManager.SpecialRoomRoll(i, coordinateData);

            coordinates = coordinateData.Keys.ToList();

            string jsonCoordinates = JsonUtility.ToJson(coordinates);
            PlayerPrefs.SetString("LevelBuild", jsonCoordinates);

            //coordinates.ForEach(a => Debug.Log("Coordinate"+a));

            coordinateData.Keys.ToList().ForEach(a => Debug.Log("Coordinate" + a));


            List<Vector2Int> seen = new();
            List<Vector2Int> directions = new List<Vector2Int> {
                new Vector2Int(0,-1), 
                new Vector2Int(0, 1),
                new Vector2Int(-1, 0),
                new Vector2Int(1, 0)};

            foreach (Vector2Int coordinate in coordinates)
            {
                Debug.Log(coordinate);



                GameObject newFloor = Instantiate(coordinateData[coordinate], Floors);
                coordinateData[coordinate] = newFloor;  
                Debug.Log("Floor Name " + coordinateData[coordinate].name);


                Vector3 dimensions = newFloor.GetComponent<MeshRenderer>().bounds.size;
                newFloor.transform.position = new Vector3(coordinate.x * dimensions.x, 0, coordinate.y * dimensions.z);


                //Debug.Log(coordinateData[coordinate].transform.position);
                foreach (Vector2Int direction in directions)
                {
                    if (seen.Contains(coordinate + direction)) { continue; }
                    else if (coordinates.Contains(coordinate + direction))
                    {
                        GameObject door = Instantiate(WallWithDoorPrefab, 
                            new Vector3(newFloor.transform.position.x + ((dimensions.x / 2) * direction.x) - (direction.x / 2f), 15, newFloor.transform.position.z + ((dimensions.z / 2) * direction.y) - (direction.y / 2f)), 
                            Quaternion.Euler(new Vector3(0, (direction.x != 0) ? 90 : 0, 0)), Doors);
                    }

                    else
                    {
                        Instantiate(WallPrefab, 
                            new Vector3(newFloor.transform.position.x + ((dimensions.x / 2) * direction.x) - (direction.x / 2f), 15, newFloor.transform.position.z + ((dimensions.z / 2) * direction.y) - (direction.y / 2f)),
                            Quaternion.Euler(new Vector3(0, (direction.y != 0) ? 90 : 0, 0)), Walls);
                    }

                }
                Instantiate(CeilingPrefab, newFloor.transform.position + (Vector3.up * 29.5f), Quaternion.identity, Ceilings);
                seen.Add(coordinate);

            }
        }

    }

    private void SpecialRoomRoll(int level)
    {


    }


    //private List<Vector2Int> 


    // Restarts the Game
    public void ResetPositions(Vector3 initPos)
    {
        GameData.Instance.Restart();

        PlayerPrefs.SetFloat("PlayerX", initPos.x);
        PlayerPrefs.SetFloat("PlayerY", initPos.y);
        PlayerPrefs.SetFloat("PlayerZ", initPos.z);
        PlayerPrefs.SetInt("PickupsCollected", 0);

        for (int i = 0; i < Enemies.Length; i++)
        {
            PlayerPrefs.SetInt("Enemy" + i + "Enabled", 1);
        }
        for (int i = 0; i < Pickups.Length; i++)
        {
            PlayerPrefs.SetInt("Pickup" + i + "Enabled", 1);
        }
    }
}
