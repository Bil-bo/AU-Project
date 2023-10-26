using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

using UnityEngine;
using System;

public class PlayerPropsRoaming : MonoBehaviour

{

    public float _speed;
    public Vector2 moveValue;
    private CharacterController charcon;
    private bool charIsGrounded;
    public MainGameManager gameManager;

    private int collectedPickups = 0;

    // Start is called before the first frame update
    void Start()
    {
        charcon = GetComponent<CharacterController>();
        collectedPickups = PlayerPrefs.GetInt("PickupsCollected");
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
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 currentMovement = (forward * moveValue.y +right * moveValue.x) * _speed * Time.deltaTime;
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
                break;
            case "Enemy":
                other.gameObject.SetActive(false);
                gameManager.EnterBattle();
                break;
        }
    }

    private void UpdatePickupText()
    {
        TextMeshProUGUI pickupText = FindFirstObjectByType<TextMeshProUGUI>();

        if(pickupText != null){
            pickupText.text = "Pickups collected: " + collectedPickups;
        }  
    }
}