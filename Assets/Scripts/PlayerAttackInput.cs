using UnityEngine;

public class PlayerAttackInput : MonoBehaviour
{
    private BattlePlayer playerCharacter;

    void Start()
    {
        playerCharacter = GetComponent<BattlePlayer>();
    }

    void Update()
    {
        if (playerCharacter.currentCard != null) //If it is this players turn, input can be recieved
        {
            HandlePlayerInput();
        }
    }

    void HandlePlayerInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Card currentAbility = playerCharacter.currentCard;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                BattleEnemy targetCharacter = hit.collider.GetComponent<BattleEnemy>();

                if (targetCharacter != null)
                {
                    // Check if the targeted character's position is within the range of the current ability
                    if (currentAbility.CheckRange(targetCharacter)) 
                    {
                        // Player has clicked on a character, initiate attack
                        currentAbility.Ability(targetCharacter);
                    }
                    else
                    {
                        targetCharacter.DisplayText("Out of Range");
                        Debug.Log("Target is out of range!");
                    }
                }
            }
        }
    }
}
