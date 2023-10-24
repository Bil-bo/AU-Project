using UnityEngine;

public class PlayerAttackInput : MonoBehaviour
{
    private TurnBasedCharacter playerCharacter;

    void Start()
    {
        playerCharacter = GetComponent<TurnBasedCharacter>();
    }

    void Update()
    {
        if (playerCharacter.isPlayerTurn) //If it is this players turn, input can be recieved
        {
            HandlePlayerInput();
        }
    }

    void HandlePlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                TurnBasedCharacter targetCharacter = hit.collider.GetComponent<TurnBasedCharacter>();

                if (targetCharacter != null)
                {
                    // Player has clicked on a character, initiate attack
                    playerCharacter.OnTargetSelected(targetCharacter);
                }
            }
        }
    }
}
