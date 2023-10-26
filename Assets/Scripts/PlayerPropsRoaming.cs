using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

using UnityEngine;

public class PlayerPropsRoaming : MonoBehaviour

{

    public float _speed;
    public Vector2 moveValue;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();

    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveValue.y, 0.0f, moveValue.x*-1);
        rb.AddForce(movement * _speed * Time.fixedDeltaTime);

    }
}
