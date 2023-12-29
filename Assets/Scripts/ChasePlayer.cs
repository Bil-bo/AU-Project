using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayer : MonoBehaviour
{
    public Transform player;
    public CharacterController controller;
    public float chaseRange = 50f;
    public float moveSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {

        //GameObject playerTag = GameObject.FindWithTag("Player");

        //if(playerTag != null)
        //{
            //player = playerTag.transform;
        //} 
        /*else{
            Debug.LogError("Player object not found with tag: Player");
        }*/
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, player.position) <= chaseRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
        
            
                
            controller.Move(moveSpeed * Time.deltaTime * direction);
                
            
        }
        
    }
}
