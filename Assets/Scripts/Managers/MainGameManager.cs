
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;


// For managing the main scene
// Slightly Deprecated by GameData
public class MainGameManager : MonoBehaviour, IOnTriggerBattle, IOnPickUpCollected, IOnLevelPassed
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
    private int PickUpsNum;

    private GameObject[] Enemies;
    private GameObject[] Pickups;
    public GameObject player;
    private static bool Init = false;




    // Start is called before the first frame update
    void Start()
    {
        // Init listeners
        EventManager.AddListener<BattleTriggerEvent>(OnTriggerBattle);
        EventManager.AddListener<PickupCollectedEvent>(OnPickUpCollected);
        EventManager.AddListener<LevelPassedEvent>(OnLevelPassed);

        Enemies = GameObject.FindGameObjectsWithTag("Enemy").OrderBy(enemy => enemy.name).ToArray();
        Pickups = GameObject.FindGameObjectsWithTag("Pickup").OrderBy(enemy => enemy.name).ToArray();
        Init = PlayerPrefs.GetInt("Init") == 1;

        if (Init)
        {
            Destroy(Addresses.gameObject);

            UnityEngine.Random.InitState(PlayerPrefs.GetInt("seed"));


            StartCoroutine(ReconstructPlayArea(PlayerPrefs.GetInt("CurrentLevel", 0), () =>
            {

                PickUpsNum = PlayerPrefs.GetInt("PickUpsNum");
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

                if (PlayerPrefs.HasKey("PlayerX"))
                {
                    Debug.Log("Setting Values");
                    float playerX = PlayerPrefs.GetFloat("PlayerX");
                    float playerY = PlayerPrefs.GetFloat("PlayerY");
                    float playerZ = PlayerPrefs.GetFloat("PlayerZ");
                    player.transform.position = new Vector3(playerX, playerY, playerZ);
                }
            }));




        }

        // Should only be called on the first load
        // Used to initialise everything
        else
        {
            int seed = (int)System.DateTime.Now.Ticks;

            UnityEngine.Random.InitState(seed);

            PlayerPrefs.SetInt("seed", seed);
            PlayerPrefs.SetInt("CurrentLevel", 0);
            PlayerPrefs.SetInt("Init", 1);
            Init = true;
            PlayerPrefs.SetInt("PickUpsNum", 2 * LevelAmount);
            GameData.Instance.AddPlayer(player.GetComponent<PlayerPropsRoaming>().BattleInfo);

            // Once the addressManager has everything ready start building the play area
            StartCoroutine(Addresses.GenerateLists());
            Addresses.ListsReady += () => StartCoroutine(VisualisePlayArea());





        }
    }

    // Start a battle
    public void OnTriggerBattle(BattleTriggerEvent eventData)
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


    // Notice a pickup
    public void OnPickUpCollected(PickupCollectedEvent eventData)
    {
        PickUpsNum -= 1;
        PlayerPrefs.SetInt("PickUpsNum", PickUpsNum);
    }


    // Move up a level
    public void OnLevelPassed(LevelPassedEvent eventData)
    {
        PlayerPrefs.SetInt("CurrentLevel", eventData.MoveToLevel);
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");
        PlayerPrefs.DeleteKey("PlayerZ");

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

    }

    // For init construction of play area
    private IEnumerator VisualisePlayArea()
    {
        // For each level
        for (int i = 0; i < LevelAmount; i++)
        {
            List<Vector2Int> coordinates = new List<Vector2Int>();
            Dictionary<Vector2Int, GameObject> coordinateData = new Dictionary<Vector2Int, GameObject>();

            // Find random pos
            int randX = UnityEngine.Random.Range(0, GridX);
            int randY = UnityEngine.Random.Range(0, GridY);

            Vector2Int startPos = new Vector2Int(randX, randY);
            coordinates.Add(startPos); // First in list is start point

            PlayerPrefs.SetString("Level" + i + startPos, LevelStart.name);

            coordinateData[startPos] = LevelStart;


            // calculate number of rooms to generate; based on Isaac
            int RoomAmount = Mathf.RoundToInt(UnityEngine.Random.Range(0, 3) + 5 + ((i + 1) * 2.5f));

            while (coordinates.Count < RoomAmount)
            {
                // Walk through level
                coordinates.AddRange(WalkerFactory.CreateWalker(new Vector2Int(GridX, GridY), RoomAmount - coordinates.Count).Walk(coordinates));
            }

            // Get FloorPrefabs from Addressables
            yield return StartCoroutine(Addresses.GetFloors(coordinates.Skip(1).ToList(), i,
                result => coordinateData = coordinateData.Concat(result).ToDictionary(a => a.Key, a => a.Value)));

            // Add special rooms
            coordinateData = SpecialRoomManager.SpecialRoomRoll(LevelAmount, i, coordinateData);

            coordinates = coordinateData.Keys.ToList();

            // Save
            string jsonCoordinates = new CoordWrapper { coordinates = coordinates }.SaveToString();
            Debug.Log("Level "+ i +" coordinates = " + jsonCoordinates);
            PlayerPrefs.SetString("LevelBuild" + i, jsonCoordinates);

            // Temp initialisation
            ConstructPlayArea(coordinateData, coordinates, i);
            yield return new WaitForSeconds(1f);

        }
        yield return null;

        // Lazy destroy and reconstruct scene to show changes
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // Instantiation of play area
    private void ConstructPlayArea(Dictionary<Vector2Int, GameObject> coordinateData, List<Vector2Int> coordinates, int level)
    {

        foreach (Vector2Int coordinate in coordinates)
        {
            // Replace prefab with instantiation
            GameObject newFloor = Instantiate(coordinateData[coordinate], Floors);
            coordinateData[coordinate] = newFloor;
        }


        // Directions for doors
        List<Vector2Int> seen = new();
        List<Vector2Int> directions = new List<Vector2Int> {
                new Vector2Int(0,-1),
                new Vector2Int(0, 1),
                new Vector2Int(-1, 0),
                new Vector2Int(1, 0)};

        foreach (Vector2Int coordinate in coordinates)
        {

            // Find dimensions
            GameObject floor = coordinateData[coordinate];
            Vector3 dimensions = floor.GetComponentInChildren<MeshRenderer>().bounds.size;
            floor.transform.position = new Vector3(coordinate.x * dimensions.x, 0, coordinate.y * dimensions.z);

            FloorManager floorPlan = floor.GetComponent<FloorManager>();
            floorPlan.Initialise(coordinate, level);

            foreach (Vector2Int direction in directions)
            {
                // If already visited floor skip
                if (seen.Contains(coordinate + direction)) { continue; }
                else if (coordinates.Contains(coordinate + direction))
                { 
                    // add door
                    GameObject door = Instantiate(WallWithDoorPrefab,
                        new Vector3(floor.transform.position.x + ((dimensions.x / 2) * direction.x) - (direction.x / 2f), 15,
                            floor.transform.position.z + ((dimensions.z / 2) * direction.y) - (direction.y / 2f)),
                            Quaternion.Euler(new Vector3(0, (direction.x != 0) ? 90 : 0, 0)), Doors);

                    floorPlan.DoorList.Add(door.GetComponent<Door>());
                    coordinateData[coordinate + direction].GetComponent<FloorManager>().DoorList.Add(door.GetComponent<Door>());
                }

                else
                {
                    // add wall
                    Instantiate(WallPrefab,
                        new Vector3(floor.transform.position.x + ((dimensions.x / 2) * direction.x) - (direction.x / 2f), 15, floor.transform.position.z + ((dimensions.z / 2) * direction.y) - (direction.y / 2f)),
                        Quaternion.Euler(new Vector3(0, (direction.y != 0) ? 90 : 0, 0)), Walls);
                }

            }

            // add ceiling
            Instantiate(CeilingPrefab, floor.transform.position + (Vector3.up * 29.5f), Quaternion.identity, Ceilings);
            seen.Add(coordinate);

        }
    }

    // Reconstruct play area from save
    private IEnumerator ReconstructPlayArea(int level, Action result)
    {
        string jsonCoords = PlayerPrefs.GetString("LevelBuild" + level);

        List<Vector2Int> coordinates = JsonUtility.FromJson<CoordWrapper>(jsonCoords).LoadFromString();

        Dictionary<Vector2Int, GameObject> RestoredData = new();
        foreach (var coord in coordinates)
        {
            var data = Addressables.LoadAssetAsync<GameObject>(PlayerPrefs.GetString("Level" + level + coord));
            yield return data;

            RestoredData[coord] = data.Result;
        }

        ConstructPlayArea(RestoredData, coordinates, level);
        yield return null;
        result.Invoke();
    }



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

    // Listener Handling
    private void OnDestroy()
    {
        EventManager.RemoveListener<BattleTriggerEvent>(OnTriggerBattle);
        EventManager.RemoveListener<PickupCollectedEvent>(OnPickUpCollected);
        EventManager.RemoveListener<LevelPassedEvent>(OnLevelPassed);
    }
}



// For saving to player prefs
[Serializable]
public class CoordWrapper
{
    public List<Vector2Int> coordinates;

    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }

    public List<Vector2Int> LoadFromString()
    {
        return coordinates;
    }
}