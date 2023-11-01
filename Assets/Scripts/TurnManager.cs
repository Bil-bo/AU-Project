using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    private List<TurnBasedCharacter> turnOrder = new List<TurnBasedCharacter>();
    private int currentIndex = 0;

    void Start()
    {
        // Delay the start of the turn sequence to give time for objects to be generated
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.1f);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Add player and enemy characters to the turn order
        foreach (var player in players)
        {
            turnOrder.Add(player.GetComponent<TurnBasedCharacter>());
        }

        foreach (var enemy in enemies)
        {
            turnOrder.Add(enemy.GetComponent<TurnBasedCharacter>());
        }

        // Start the turn sequence
        StartCoroutine(TurnSequence());
    }

    IEnumerator TurnSequence()
    {
        while (true)
        {
            // Check if there are any characters in the turn order
            if (turnOrder.Count > 0)
            {
                // Wait for the current character to finish its turn
                if ( turnOrder[currentIndex] != null )
                {
                    yield return StartCoroutine(turnOrder[currentIndex].TakeTurn());
                    // Delay between turns
                    yield return new WaitForSeconds(1f);
                }
                

                // Move to the next character in the turn order
                currentIndex = (currentIndex + 1) % turnOrder.Count;   
            }
            else
            {
                Debug.LogWarning("No characters in the turn order.");
                yield return null;
            }
        }
    }
}
