/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInScene : MonoBehaviour
{
    public bool isPressing { get; set; } = false;
    public BattlePlayer Player;
    public Card card;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsPointerOverCard() && Player.isMyTurn)
        {
            isPressing = true;
            card.Select();
        }
        else if (Input.GetMouseButtonUp(0) && isPressing)
        {
            isPressing = false;
            card.Deselect();
        }
    }

    private bool IsPointerOverCard()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            return hitInfo.collider.gameObject == gameObject;
        }

        return false;
    }
    public void Select()
    {
        Player.currentCard = this;
    }

    public void Deselect()
    {
        Player.currentCard = null;
    }
}
*/