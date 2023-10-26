using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

using UnityEngine;
using System;

public class PlayerPropsRoaming : MonoBehaviour

{

    public float _speed;
    public Vector2 moveValue;
    
    private Vector3 initialPlayerPosition;
    private CharacterController charcon;
    private bool charIsGrounded;
    public MainGameManager gameManager;

    private int collectedPickups = 0;
    public int maxPickups = 3;
    private bool hasWon = false;

    private List<Vector3> initialEnemyPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        charcon = GetComponent<CharacterController>();
        collectedPickups = PlayerPrefs.GetInt("PickupsCollected");
        initialPlayerPosition = transform.position;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            initialEnemyPositions.Add(enemy.transform.position);
        }
        UpdatePickupText();
    }

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();

    }

    void Update()
    {
        if (charcon)
        {
            handleMovement();
            charIsGrounded = charcon.isGrounded;
        }

    }

    void handleMovement()
    {

        var camera = Camera.main;

        var forward = camera.transform.forward;
        var right = camera.transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;
        forward.Normalize();
        right.Normalize();

        Vector3 currentMovement = (forward * moveValue.y +right * moveValue.x) * _speed * Time.deltaTime;
        currentMovement.y = charIsGrounded ? 0.0f : -1.0f;
        Physics.SyncTransforms(); //REQUIRED - allows gamecontroller to move player
        charcon.Move(currentMovement);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Pickup":
                other.gameObject.SetActive(false);
                collectedPickups++;
                PlayerPrefs.SetInt("PickupsCollected", collectedPickups);
                UpdatePickupText();

                if(collectedPickups >= maxPickups){
                    hasWon = true;
                    DisplayWinMessage();
                }
                break;
            case "Enemy":
                other.gameObject.SetActive(false);
                gameManager.EnterBattle();
                break;
        }
    }

    private void UpdatePickupText()
    {
        TextMeshProUGUI pickupText = GameObject.FindGameObjectWithTag("PickupText")?.GetComponent<TextMeshProUGUI>();

        if(pickupText != null){
            pickupText.text = "Pickups collected: " + collectedPickups;
        }  
    }

    private void DisplayWinMessage()
    {
        

        if(hasWon){
            TextMeshProUGUI winText = GameObject.FindGameObjectWithTag("WinText")?.GetComponent<TextMeshProUGUI>();

            if(winText != null)
            {
                winText.text = "You won!";

                transform.position = initialPlayerPosition;
                collectedPickups = 0;
                PlayerPrefs.SetInt("PickupsCollected", 0);
                RespawnEnemies();

                SceneManager.LoadScene("Menu");


                
            }
        }
    }

    private void RespawnEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

    // Iterate through the enemy game objects and set their positions to their initial spawn positions.
        for (int i = 0; i < enemies.Length; i++)
        {
            if (i < initialEnemyPositions.Count)
            {
                enemies[i].transform.position = initialEnemyPositions[i];
            }
        }
    }
}