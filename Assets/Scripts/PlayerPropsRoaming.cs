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

    private CharacterController charcon;
    private bool charIsGrounded;
    public MainGameManager gameManager;
    public GameObject pause;

    private int collectedPickups = 0;
    public int maxPickups = 3;
    private bool hasWon = false;

    // Start is called before the first frame update
    void Start()
    {
        charcon = GetComponent<CharacterController>();
        collectedPickups = PlayerPrefs.GetInt("PickupsCollected");
        pause.SetActive(false);
        UpdatePickupText();
    }

    void OnPause()
    {
        pause.SetActive(true);
        Time.timeScale = 0;

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
                
                if (other.gameObject.GetComponent<PickUpsData>().getDoor() != null)
                {
                    other.gameObject.GetComponent<PickUpsData>().getDoor().SetActive(false);       
                }
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
                gameManager.Reset(new Vector3(4,4,4));
                collectedPickups = 0;
                SceneManager.LoadScene("Menu");


                
            }
        }
    }
}