using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
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
        Debug.Log("We here?");
        
        if (Player.isMyTurn)
        {
            Debug.Log("We here here?");
            isPressing = true;

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("We Out");
        Debug.Log(Player.isMyTurn);
        Debug.Log(Player.isMyTurn && isPressing);
        if (Player.isMyTurn && isPressing)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
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
    }

    private bool IsPointerOverCard()
    {
        Debug.Log("Ok we in this");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.Log("Ok we in this now");
            Debug.Log(hitInfo.collider.gameObject == gameObject);
            return hitInfo.collider.gameObject == gameObject;
        }

        return false;
    }


    private bool CheckIfTarget(RaycastHit hit)
    {
        Debug.Log("We Checkin");

        Debug.Log(nameof(CardDataHolder.Targets));
        Debug.Log(hit.collider.CompareTag(CardDataHolder.Targets.ToString()));


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
            effect.Effect(target);
        }       
    }
}
