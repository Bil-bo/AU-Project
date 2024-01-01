using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;
using UnityEditor;



// Sets up and starts the main battle scene
public class BattleManager : MonoBehaviour, IOnPlayerDeath, IOnEnemyDeath
{
    //public List<GameObject> characters = new List<GameObject>();

    public List<BattlePlayer> players = new List<BattlePlayer>();
    public List<BattleEnemy> enemies = new List<BattleEnemy>();
    private List<BattlePlayer> DeadPlayers = new List<BattlePlayer>();
    private List<BattleEnemy> DeadEnemies = new List<BattleEnemy>();

    private BattlePlayer CurrentPlayer;

    private int EnemyCount = 0;
    private int PlayerCount = 0;
    private bool playerWin = false;
    private bool playerLose = false;
    private bool isPressing = false;

    public GameManager manager;

    private PlayerInput inputs;


    [SerializeField]
    private GameObject decks;

    [SerializeField]
    private ArrowPointer Line;

    [SerializeField]
    private GameObject StatusFieldPrefab;

    [SerializeField]
    private GameObject HealthBarPrefab;


    [SerializeField]
    private List<GameObject> PlayerPositions = new List<GameObject>();

    [SerializeField]
    private List<GameObject> EnemyPositions = new List<GameObject>();

    [SerializeField]
    private List<Material> TargetMats = new List<Material>();

    private DeckHandler deckHandler;

    private GameObject _MainCard;
    private Card MainCardData;

    private GameObject _SecondaryCard;
    private Card SecondaryCardData;

    private Vector2 mousePrevPos;
    private bool InPlay;

    private bool TargetSelection = false;
    private BaseBattleCharacter SingleTarget;


    public GameObject MainCard { 
        get { return _MainCard; }
        set { _MainCard = value;
            if (value != null) { MainCardData = value.GetComponentInChildren<Card>(); }
            else { MainCardData = null; }
        }
    }

    public GameObject SecondaryCard
    {
        get { return _SecondaryCard; }
        set
        {
            _SecondaryCard = value;
            if (value != null) { SecondaryCardData = value.GetComponentInChildren<Card>(); }
            else { SecondaryCardData = null; }
        }
    }

    private void Awake()
    {
        inputs = GetComponent<PlayerInput>();
        EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);
        EventManager.AddListener<EnemyDeathEvent>(OnEnemyDeath);
    }

    void Start()
    {

        var clickInput = inputs.actions["UI/Click"];
        clickInput.started += Click;
        clickInput.canceled += ClickRelease;


        deckHandler = decks.GetComponent<DeckHandler>();
        manager = FindAnyObjectByType<GameManager>();

        List<GameObject> allPostions = PlayerPositions.Concat(EnemyPositions).ToList();
        foreach (GameObject position in allPostions)
        {
            position.transform.GetChild(0).gameObject.SetActive(false);
        }

        GeneratePlayers();
        GenerateEnemies();
        

        //Unlock mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(StartBattlePhase());
    }


    void Click(InputAction.CallbackContext context)
    {
        // Tell the deck handler to check if a card is being selected
        // if a card has been selected have it ready here.
        // Start drawing a line
        // start highlighting potential targets
        Vector2 mousePos = inputs.actions["UI/Point"].ReadValue<Vector2>();
        GameObject newCard;
        if (deckHandler.SelectCard(mousePos, out newCard) )
        {
            MainCard = newCard;
            isPressing = true;
            mousePrevPos = mousePos;    
            Line.SetStartPos(MainCard.GetComponent<RectTransform>());

        }
    }

    void ClickHold()
    {
        if (mousePrevPos != inputs.actions["UI/Point"].ReadValue<Vector2>())
        {
            mousePrevPos = inputs.actions["UI/Point"].ReadValue<Vector2>();

            Ray ray = Camera.main.ScreenPointToRay(mousePrevPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 45f, LayerMask.GetMask("PlayArea")))
            {
                SecondaryCard = null;
                InPlay = true;
                Line.ShowArrow(true);
                Line.DrawArrow(mousePrevPos);
                HighlightTargets(MainCardData.Target, MainCardData.Range, CurrentPlayer);
                if (TargetSelection && Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
                {
                    if (hit.collider.gameObject.TryGetComponent<BaseBattleCharacter>(out BaseBattleCharacter hitCharacter))
                    {
                        if (hitCharacter.CanSelect)
                        {
                            SingleTarget = hitCharacter;
                            SingleTarget.Targeter.GetComponent<Renderer>().material = TargetMats[2];
                        }
                    }
                }
                else { SingleTarget = null; }
            }
            else if (deckHandler.SelectCard(mousePrevPos, out _SecondaryCard))
            {
                InPlay = false;
                DeselectAll();
                Line.ShowArrow(false);
                SecondaryCard = _SecondaryCard;
                if (SecondaryCardData.CanMerge(MainCardData))
                {
                    // Highlight it, or something
                }
            }

            else 
            {
                SecondaryCard = null;
                InPlay = false;
                DeselectAll();
                Line.ShowArrow(false);
            }
        }
    }

    void ClickRelease(InputAction.CallbackContext context)
    {
        if (isPressing && InPlay && !(MainCardData.Cost > CurrentPlayer.CurrentEnergy)) 
        {
            List<BaseBattleCharacter> targets = GetTargets(MainCardData.Target, MainCardData.Range);
            if (targets.Count > 0) 
            {
                MainCardData.Use(CurrentPlayer, targets);
                deckHandler.UseCard(MainCard, MainCardData);
                CurrentPlayer.CurrentEnergy -= (MainCardData.Cost == -1) ? CurrentPlayer.CurrentEnergy : MainCardData.Cost;
            }

        }

        else if (SecondaryCard != null)
        {

            if (deckHandler.TryMergeCards(MainCardData, SecondaryCardData))
            { CurrentPlayer.CurrentEnergy -= 1; }
            SecondaryCard = null;
        }

        isPressing = false;
        TargetSelection = false;
        SingleTarget = null;
        MainCard = null;
        Line.ShowArrow(false);
        DeselectAll();
    }


    void GenerateEnemies()
    {
        var enemyInput = GameData.Instance.battleEnemies;


        for (int i = 0; i < enemyInput.Length; i++)
        {
            // Instantiate the enemy prefab

            GameObject enemyBody = Instantiate(enemyInput[i]);
            
            //Need to add the type of enemy from enemyInput list to the enemy GameObject
            BattleEnemy enemy = enemyBody.GetComponent<BattleEnemy>();

            Instantiate(StatusFieldPrefab, enemyBody.transform);
            HealthBar healthBar = Instantiate(HealthBarPrefab, enemy.transform).GetComponent<HealthBar>();
            healthBar.Initialise(enemy.CharID, enemy.maxHealth, enemy.CurrentHealth);

            // Assign a unique name to the enemy
            enemy.name = "Enemy " + enemy.EnemyName + (EnemyCount + 1);
            enemy.PositionMarker = EnemyPositions[i];
            EnemyCount++;

            // SET OTHER PROPERTIES OR COMPONENTS FOR ENEMY HERE
            enemies.Add(enemy);
        }
    }


    void GeneratePlayers()
    {
        List<GameObject> party = GameData.Instance.BattlePlayers.Keys.ToList();
        for (int i = 0; i < party.Count; i++)
        { 
            BattlePlayer playerData = party[i].GetComponent<BattlePlayer>();
            Instantiate(StatusFieldPrefab, playerData.transform);
            HealthBar healthBar = Instantiate(HealthBarPrefab, playerData.transform).GetComponent<HealthBar>();
            healthBar.Initialise(playerData.CharID, playerData.maxHealth, playerData.CurrentHealth);

            deckHandler.AddDeck(party[i]);
            party[i].SetActive(true);
            playerData.PositionMarker = PlayerPositions[i];
            PlayerCount++;
            players.Add(playerData);

        }

    }


    private void Update()
    {
        if (isPressing) { ClickHold(); }
        
    }

    IEnumerator StartBattlePhase()
    {
        while(!playerWin || !playerLose) {

            foreach (BattlePlayer player in players)
            {
                if (playerWin || playerLose || DeadPlayers.Contains(player)) { continue; }
                else
                { 
                    CurrentPlayer = player;
                    CurrentPlayer.Targeter.SetActive(true);
                    CurrentPlayer.Targeter.GetComponent<Renderer>().material = TargetMats[0];

                    StartOfTurnEvent gameEvent = new StartOfTurnEvent()
                    {
                        CharacterID = CurrentPlayer.CharID,
                        Character = player
                    };

                    EventManager.Broadcast(gameEvent);

                    if (DeadPlayers.Contains(player)) { continue; }
                    else
                    {
                        deckHandler.currentPlayer = CurrentPlayer.gameObject;
                        deckHandler.ShowDeck();
                        yield return new WaitForSeconds(0.5f);
                        yield return StartCoroutine(player.DoTurn());
                        CurrentPlayer.Targeter.SetActive(false);
                        deckHandler.HideDeck();
                    }
                }
            }


            foreach (BattleEnemy enemy in enemies)
            {
                if (playerWin || playerLose || DeadEnemies.Contains(enemy)) { continue; }
                else
                {
                    StartOfTurnEvent gameEvent = new StartOfTurnEvent()
                    {
                        CharacterID = enemy.CharID,
                        Character = enemy
                    };
                    EventManager.Broadcast(gameEvent);
                    if (DeadEnemies.Contains(enemy)) { continue; }
                    else
                    {
                        yield return StartCoroutine(enemy.DoTurn());
                        yield return new WaitForSeconds(1f);
                    }

                }
            }
            Debug.Log(DeadEnemies.Count);
            if (DeadPlayers.Count > 0 || DeadEnemies.Count > 0) { CleanUp(); }
        }



    }

    IEnumerator ExitBattle(bool hasWon)
    {
        CleanUp();
        if (hasWon)
        {
            manager.ShowOverlay("You Won!");
            
            yield return new WaitForSeconds(2f);

            foreach (BattlePlayer player in players)
            {
                player.transform.DetachChildren();
                player.transform.parent = GameData.Instance.transform;
            }

            deckHandler.ResetCards();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        else
        {
            manager.ShowOverlay("You Lost!");
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(0);
        }
    }


    private List<BaseBattleCharacter> GetTargets(Target targets, int range)
    {
        List<BaseBattleCharacter> toReturn = new();

        switch (targets)
        {
            case Target.PLAYER:
            case Target.ENEMY:
            case Target.PLAYERS_OR_ENEMIES:
                if (SingleTarget != null) { toReturn.Add(SingleTarget); }
                return toReturn;

            case Target.ALL_PLAYERS:
                return new List<BaseBattleCharacter>(players);        

            case Target.SELF_AND_ENEMIES:
                toReturn.Add(CurrentPlayer);
                toReturn.AddRange(enemies);
                return toReturn;

            case Target.ALL_ENEMIES:
                toReturn.AddRange(enemies);
                return new List<BaseBattleCharacter>(enemies);    
                
            case Target.ALL:
                toReturn.AddRange(players);
                toReturn.AddRange(enemies);
                return toReturn;

            case Target.PLAYERS_IN_RANGE:
                foreach (BattlePlayer player in players)
                {
                    if (player.Position >= Mathf.Clamp(CurrentPlayer.Position - range, 0, 10) &&
                        player.Position <= Mathf.Clamp(CurrentPlayer.Position + range, 0, players.Count))
                    {
                        toReturn.Add(player);
                    }
                }
                return toReturn;


            case Target.ENEMIES_IN_RANGE:
                foreach (BattlePlayer player in players)
                {
                    if (player.Position >= Mathf.Clamp(CurrentPlayer.Position - range, 0, 10) &&
                        player.Position <= Mathf.Clamp(CurrentPlayer.Position + range, 0, players.Count))
                    {
                        toReturn.Add(player);
                    }
                }
                return toReturn;


            case Target.ALL_IN_RANGE:
                foreach (BattleEnemy enemy in enemies)
                {
                    if (range - (CurrentPlayer.Position + 1) >= enemy.Position)
                    {
                        toReturn.Add(enemy);
                    }
                }
                return toReturn;

            
            default:
                return toReturn;

        }
    }



    private void HighlightTargets (Target targets, int range, BattlePlayer comparePlayer)
    {
        switch (targets)
        {
            case Target.PLAYER:
                TargetSelection = true;
                SetHighlights(new List<BaseBattleCharacter>(players), comparePlayer, 1, true, range, true);
                break;

            case Target.ENEMY:
                TargetSelection = true;
                SetHighlights(new List<BaseBattleCharacter>(enemies), comparePlayer, 1, true, range, false, true);
                break;

            case Target.SELF:
                comparePlayer.CanSelect = true;
                comparePlayer.Targeter.GetComponent<Renderer>().material = TargetMats[2];
                break;

            case Target.ALL:
                List<BaseBattleCharacter> all = new List<BaseBattleCharacter>();
                all.AddRange(players);
                all.AddRange(enemies);
                SetHighlights(all, comparePlayer, 2);
                break;

            case Target.ALL_ENEMIES:
                SetHighlights(new List<BaseBattleCharacter>(enemies), comparePlayer, 2);
                break;

            case Target.ALL_PLAYERS:
                SetHighlights(new List<BaseBattleCharacter>(players), comparePlayer, 2);
                break;

            case Target.ALL_IN_RANGE:
                SetHighlights(new List<BaseBattleCharacter>(players), comparePlayer, 2, true, range, true, false);
                SetHighlights(new List<BaseBattleCharacter>(enemies), comparePlayer, 2, true, range, false, true);
                break;

            case Target.SELF_AND_ENEMIES:
                comparePlayer.CanSelect = true;
                comparePlayer.Targeter.GetComponent<Renderer>().material = TargetMats[2];
                SetHighlights(new List<BaseBattleCharacter>(enemies), comparePlayer, 2);
                break;

            case Target.PLAYERS_IN_RANGE:
                SetHighlights(new List<BaseBattleCharacter>(players), comparePlayer, 2, true, range, true);
                break;

            case Target.ENEMIES_IN_RANGE:
                SetHighlights(new List<BaseBattleCharacter>(enemies), comparePlayer, 1, true, range, false, true);
                break;

            case Target.PLAYERS_OR_ENEMIES:
                TargetSelection = true;
                SetHighlights(new List<BaseBattleCharacter>(players), comparePlayer, 1, true, range, true, false);
                SetHighlights(new List<BaseBattleCharacter>(enemies), comparePlayer, 1, true, range, false, true);
                break;

            default:
                break;
        }
    }

    private void SetHighlights(List<BaseBattleCharacter> charToHighlight, BattlePlayer comparePlayer, int MatIndex, bool setActive = true, int range = 0, bool playerRange = false, bool enemyRange = false)
    {
        foreach (BaseBattleCharacter character in charToHighlight)
        {
            character.Targeter.SetActive(setActive);
            if (playerRange) 
            {
                if (character.Position >= Mathf.Clamp(comparePlayer.Position - range, 0, 10) && 
                    character.Position <= Mathf.Clamp(comparePlayer.Position + range, 0, players.Count))
                {
                    character.CanSelect = setActive;
                    character.Targeter.GetComponent<Renderer>().material = TargetMats[MatIndex];
                }
            }
            else if (enemyRange)
            {
                if (range - (comparePlayer.Position + 1) >= character.Position)
                {
                    character.CanSelect = setActive;
                    character.Targeter.GetComponent<Renderer>().material = TargetMats[MatIndex];
                }
            }
            else 
            {
                character.CanSelect = setActive;
                character.Targeter.GetComponent<Renderer>().material = TargetMats[MatIndex];

            }

        }
    }


    private void DeselectAll() 
    {
        List<BaseBattleCharacter> all = new List<BaseBattleCharacter>();
        all.AddRange(players);
        all.AddRange(enemies);
        all.Remove(CurrentPlayer);
        SetHighlights(all, CurrentPlayer, 3, false);
        CurrentPlayer.Targeter.GetComponent<Renderer>().material = TargetMats[0];
        CurrentPlayer.CanSelect = false;
    }

    public void OnPlayerDeath(PlayerDeathEvent eventData)
    {
        BattlePlayer deadPlayer = eventData.player;
        DeadPlayers.Add(deadPlayer);
        PlayerCount--;

        if (PlayerCount == 0)
        {
            StopCoroutine(StartBattlePhase());
            StartCoroutine(ExitBattle(false));
            // Game Over Lose Function call here, break out of the coroutine
        }

        else
        {

            deadPlayer.PositionMarker = null;
            deadPlayer.transform.SetParent(transform, false);
            deadPlayer.gameObject.SetActive(false);

            int position = deadPlayer.Position;



            foreach (BattlePlayer player in players)
            {
                if (player == deadPlayer) continue;
                else { if (player.Position > position) { player.PositionMarker = PlayerPositions[player.Position - 1]; } }
            }
        }
    }

    public void OnEnemyDeath(EnemyDeathEvent eventData)
    {
        BattleEnemy deadEnemy = eventData.enemy;
        DeadEnemies.Add(deadEnemy);
        EnemyCount--;

        if (EnemyCount == 0)
        {
            StopCoroutine(StartBattlePhase());
            StartCoroutine(ExitBattle(true));
        }

        else
        {
            deadEnemy.PositionMarker = null;
            deadEnemy.transform.SetParent(transform, false);
            deadEnemy.gameObject.SetActive(false);

            int position = deadEnemy.Position;
            foreach (BattleEnemy enemy in enemies)
            {
                if (enemy == deadEnemy) continue;
                else { if (enemy.Position > position) { enemy.PositionMarker = EnemyPositions[enemy.Position - 1]; } }
            }
        }
    }

    private void CleanUp()
    {
        

        for (int i = DeadPlayers.Count - 1; i >= 0; i--)
        {
            BattlePlayer deadPlayer = DeadPlayers[i];
            DeadPlayers.RemoveAt(i);  // Remove from DeadPlayers first
            players.Remove(deadPlayer);  // Remove from players
            Destroy(deadPlayer.gameObject);
        }

        // Clean up dead enemies
        for (int i = DeadEnemies.Count - 1; i >= 0; i--)
        {
            BattleEnemy deadEnemy = DeadEnemies[i];
            DeadEnemies.RemoveAt(i);  // Remove from DeadEnemies first
            enemies.Remove(deadEnemy);  // Remove from enemies
            Destroy(deadEnemy.gameObject);
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
        EventManager.RemoveListener<EnemyDeathEvent>(OnEnemyDeath);
    }


}
