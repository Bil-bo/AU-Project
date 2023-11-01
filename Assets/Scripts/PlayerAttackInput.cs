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
        if (playerCharacter.currentSelectedAbilityAction != null) //If it is this players turn, input can be recieved
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
                BattleEnemy targetCharacter = hit.collider.GetComponent<BattleEnemy>();

                if (targetCharacter != null)
                {
                    // Assuming playerCharacter.CurrentAbility is the ability they want to use
                    AbilityCard currentAbility = playerCharacter.currentCard;

                    //if (targetCharacter.gameObject.tag == "Player")
                    //{
                        //targetCharacter.DisplayText("Cannot attack ally");
                        //Debug.Log("Cannot attack ally");
                    //}

                    // Check if the targeted character's position is within the range of the current ability
                    if (targetCharacter.Position <= currentAbility.Range)
                    {
                        // Player has clicked on a character, initiate attack
                        playerCharacter.OnTargetSelected(targetCharacter);
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
