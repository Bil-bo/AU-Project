using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{


    public BattlePlayer Player;

    private bool isPressing = false;

    public CardData CardDataHolder;

    private void Start()
    {

        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = CardDataHolder.CardName;
    }
    
    public void OnPointerDown(PointerEventData eventData) 
    {

        if (Player.isMyTurn)
        {

            isPressing = true;
            Select();

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {



        if (Player.isMyTurn && isPressing)
        {

            try {

                GraphicRaycaster raycaster = gameObject.transform.parent.GetComponent<GraphicRaycaster>();
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(eventData, results);
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
      
            } catch { }
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

    public void Ability(BaseBattleCharacter target)
    {
        foreach(CardActions effect in CardDataHolder.Effects)
        {
            effect.Effect(target, Player);
        }       
    }

    public void combine(GameObject cardToCombine)
    {
        Debug.Log("Will They Combine?");
        GameObject combinationHolder;
        Debug.Log(CardDataHolder.Combinations.ContainsKey(cardToCombine)); 
        Debug.Log(cardToCombine.GetComponent<Card>().CardDataHolder.Combinations.ContainsKey(CardDataHolder.gameObject));
        if (CardDataHolder.Combinations.ContainsKey(cardToCombine.GetComponent<Card>().CardDataHolder.gameObject))
        {
            Debug.Log("They Combine Here");
            combinationHolder = CardDataHolder.Combinations[cardToCombine.GetComponent<Card>().CardDataHolder.gameObject];                     
        
        }

        else if (cardToCombine.GetComponent<Card>().CardDataHolder.Combinations.ContainsKey(CardDataHolder.gameObject))
        {
            Debug.Log("They Combine Here Actually");
            combinationHolder = cardToCombine.GetComponent<Card>().CardDataHolder.Combinations[CardDataHolder.gameObject];
        }
        else
        {
            Debug.Log("They Don't combine :(");
            combinationHolder = null;
        }

        if (combinationHolder != null)
        {
            GameObject[] cardsToPass = new GameObject[] { gameObject, cardToCombine};
            gameObject.transform.parent.gameObject.GetComponent<DeckHandler>().AddCard(combinationHolder, cardsToPass);

        }

    }
}
