using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// ):
// Script Attached Directly to All Card Instances
public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    // Player the card belongs to
    public BattlePlayer Player;

    // If this card was held down on first, this is true
    private bool isPressing = false;

    // The information of the card. Includes its effects
    public CardData CardDataHolder;

    // UNique ID for all card instances. For combining cards
    public Guid CardID { get; } = Guid.NewGuid();


    // Sets the card name when it is created
    private void Start()
    {

        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = CardDataHolder.CardName;
    }
    

    // When the card is pressed down on
    public void OnPointerDown(PointerEventData eventData) 
    {

        if (Player.isMyTurn)
        {

            isPressing = true;
            Select();

        }
    }

    // This one
    // When The mouse is released
    // Called from the same card that the original OnPointerDown was called on, not the card the mouse was released over
    public void OnPointerUp(PointerEventData eventData)
    {



        if (Player.isMyTurn && isPressing)
        {
            // Try Catch Block For Checking for combinable cards
            try {
                GraphicRaycaster raycaster = gameObject.transform.parent.GetComponent<GraphicRaycaster>();
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(eventData, results);
                // Checks if the Raycast hit another card
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.GetComponent<Card>() != null)
                    {
                        GameObject Othercard= result.gameObject;
                        if (Othercard != gameObject)
                        {
                            combine(Othercard);
                        }
                    }

                }
      
            } catch (Exception e)
            {
                Debug.LogError(e);            
            }
            // Check If The Card has hit the appropriate target, and execute effects
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.name);
                Debug.Log("We Look In Here?");
                if (CheckIfTarget(hit))
                {
                    BaseBattleCharacter target = hit.collider.GetComponent<BaseBattleCharacter>();
                    if (CheckRange(target))
                    {
                        Ability(target);
                        Player.FinishTurn();
                    }
                }
            }
        }
        isPressing=false;
        Deselect();

    }


    // Check if the card has hit the correct target
    private bool CheckIfTarget(RaycastHit hit)
    {
        if (hit.collider.GetComponent<Card>() != null)
        {
            Debug.Log(hit.collider.GetComponent<Card>().CardDataHolder.CardName);
        }
        return (hit.collider.CompareTag(CardDataHolder.Targets.ToString()));
    }

    public void Select()
    {
        Player.currentCard = this;
    }

    public void Deselect()
    {
        Player.currentCard = null;
    }
    
    public bool CheckRange(BaseBattleCharacter target)
    {
        return (target.Position <= CardDataHolder.Range);
    }

    // Activates all card effects
    public void Ability(BaseBattleCharacter target)
    {
        foreach(CardActions effect in CardDataHolder.Effects)
        {
            effect.Effect(target, Player);
        }       
    }

    // Main Game Core Concept: Combining Cards - Two Compatible cards come together to create one stronger card
    public void combine(GameObject cardToCombine)
    {
        // Have to check via the cardDataHolders gameObject since that is the one with a different name
        // Alternative Setup? : Use String name or String type as identifier


        GameObject combinationHolder;
        Debug.Log(cardToCombine.GetComponent<Card>().CardDataHolder.Combinations.ContainsKey(CardDataHolder.gameObject));
        if (CardDataHolder.Combinations.ContainsKey(cardToCombine.GetComponent<Card>().CardDataHolder.gameObject))
        {

            combinationHolder = CardDataHolder.Combinations[cardToCombine.GetComponent<Card>().CardDataHolder.gameObject];                     
        
        }

        else if (cardToCombine.GetComponent<Card>().CardDataHolder.Combinations.ContainsKey(CardDataHolder.gameObject))
        {

            combinationHolder = cardToCombine.GetComponent<Card>().CardDataHolder.Combinations[CardDataHolder.gameObject];
        }
        else
        {
            combinationHolder = null;
        }


        // If the cards can combine add them to the deckHandler
        if (combinationHolder != null)
        {
            GameObject[] cardsToPass = new GameObject[] { gameObject, cardToCombine};
            gameObject.transform.parent.gameObject.GetComponent<DeckHandler>().AddCard(combinationHolder, cardsToPass);

        }

    }
}
