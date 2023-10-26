using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

using UnityEngine;

public class PlayerPropsRoaming : MonoBehaviour

{

    public float _speed;
    public Vector2 moveValue;
    private CharacterController charcon;
    private bool charIsGrounded;

    // Start is called before the first frame update
    void Start()
    {
        charcon = GetComponent<CharacterController>();
    }

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();

    }

    void Update()
    {
        handleMovement();

    }

    void handleMovement()
    {
        Vector3 currentMovement = new Vector3(moveValue.y, charIsGrounded ? 0.0f:-1.0f, moveValue.x*-1) * _speed * Time.deltaTime;
        charcon.Move(currentMovement);
    }
}
