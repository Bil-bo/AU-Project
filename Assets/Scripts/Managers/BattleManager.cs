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
public class BattleManager : MonoBehaviour
{
    //public List<GameObject> characters = new List<GameObject>();

    public List<BattlePlayer> players = new List<BattlePlayer>();
    public List<BattleEnemy> enemies = new List<BattleEnemy>();

    private BattlePlayer CurrentPlayer;

    private int enemyCount = 0;
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

    private GameObject _Card;
    private Card CardData;

    private Vector2 mousePrevPos;
    private bool InPlay;

    private bool TargetSelection = false;
    private BaseBattleCharacter SingleTarget;


    public GameObject Card { 
        get { return _Card; }
        set { _Card = value;
            if (value != null) { CardData = value.GetComponentInChildren<Card>(); }
            else { CardData = null; }
        }
    }

    private void Awake()
    {
        inputs = GetComponent<PlayerInput>();
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
        Debug.Log(mousePos);
        Card = deckHandler.SelectCard(mousePos);
        if (Card != null )
        {
            isPressing = true;
            Debug.Log(CardData.Name);
            mousePrevPos = mousePos;    
            Line.SetStartPos(Card.GetComponent<RectTransform>());

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
                InPlay = true;
                Line.ShowArrow(true);
                Line.DrawArrow(mousePrevPos);
                HighlightTargets(CardData.Target, CardData.Range, CurrentPlayer);
                Debug.Log(TargetSelection);
                if (TargetSelection && Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
                {
                    Debug.Log(hit.collider.gameObject.name);
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

            else 
            {
                InPlay = false;
                DeselectAll();
                Line.ShowArrow(false);
            }
        }
    }

    void ClickRelease(InputAction.CallbackContext context)
    {
        if (isPressing && InPlay && !(CardData.Cost > CurrentPlayer.CurrentEnergy)) 
        {
            List<BaseBattleCharacter> targets = GetTargets(CardData.Target, CardData.Range);
            if (targets.Count > 0) 
            {
                CardData.Use(CurrentPlayer, targets);
                CurrentPlayer.CurrentEnergy -= (CardData.Cost == -1) ? CurrentPlayer.CurrentEnergy : CardData.Cost;
            }

        }

        isPressing = false;
        TargetSelection = false;
        SingleTarget = null;
        Card = null;
        Line.ShowArrow(false);
        DeselectAll();
        Debug.Log("Button Released");
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
            enemy.name = "Enemy " + enemy.EnemyName + (enemyCount + 1);
            enemy.PositionMarker = EnemyPositions[i];
            enemyCount++;

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
                if (playerWin || playerLose) { continue; }
                else
                { 
                    CurrentPlayer = player;
                    CurrentPlayer.Targeter.SetActive(true);
                    CurrentPlayer.Targeter.GetComponent<Renderer>().material = TargetMats[0];
                    deckHandler.currentPlayer = CurrentPlayer.gameObject;
                    deckHandler.ShowDeck();
                    yield return new WaitForSeconds(0.5f);

                    StartOfTurnEvent gameEvent = new StartOfTurnEvent()
                    {
                        CharacterID = CurrentPlayer.CharID,
                        Character = player
                    };
                    EventManager.Broadcast(gameEvent);

                    yield return StartCoroutine(player.DoTurn());
                    CheckEnemyDeaths();
                    CheckPlayerDeaths();
                    CurrentPlayer.Targeter.SetActive(false);
                    deckHandler.HideDeck();
                }
            }


            foreach (BattleEnemy enemy in enemies)
            {
                if (playerWin || playerLose) { continue; }
                else
                {
                    StartOfTurnEvent gameEvent = new StartOfTurnEvent()
                    {
                        CharacterID = enemy.CharID,
                        Character = enemy
                    };
                    EventManager.Broadcast(gameEvent);

                    yield return StartCoroutine(enemy.DoTurn());
                    CheckPlayerDeaths();
                    CheckEnemyDeaths();
                    yield return new WaitForSeconds(1f);

                }
            }
        }

        if (playerWin)
        {
            manager.ShowOverlay("You Won!");
            yield return new WaitForSeconds(2f);
            ExitBattle();
        }

        // Check if all players are defeated
        else if (playerLose)
        {
            manager.ShowOverlay("You Lost!");
        }
    }

    void CheckPlayerDeaths(){
        for(int i= 0; i < players.Count; i++) 
        {
            if(players[i].GetComponent<BattlePlayer>().dead)
            {
                var player = players[i];
                players.RemoveAt(i);
                Destroy(player);
            }
        }
        playerLose = (players.Count == 0);

    }

    void CheckEnemyDeaths()
    {
        for(int i=0; i<enemies.Count;i++)
        {
            if(enemies[i].GetComponent<BattleEnemy>().dead)
            {
                var enemy = enemies[i];
                enemies.RemoveAt(i);
                Destroy(enemy);
            }
        }
        playerWin = (enemies.Count == 0);
    }

    public void ExitBattle()
    {
        foreach (BattlePlayer player in players)
        {
            player.transform.parent = GameData.Instance.transform;
        }

        deckHandler.ResetCards();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
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


}
