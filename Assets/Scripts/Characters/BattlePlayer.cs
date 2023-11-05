using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEditor;
using System.Runtime.CompilerServices;

public class BattlePlayer : BaseBattleCharacter
{

    public Card currentCard { get; set; }
    public GameObject deckViewer;
    private DeckHandler deckHandler;
    public bool isMyTurn = false;
    public int maxHand { get; set; } = 3;



    private void CreateCanvas()
    {
        
        deckHandler = Instantiate(deckViewer, deckViewer.transform.position, Quaternion.identity).GetComponent<DeckHandler>();
        deckHandler.initialise(this);

    }


    // Awake called to avoid race conditions with the coroutine
    private void Awake()
    {
        CreateCanvas();
        hudManager = GameObject.Find("Canvas").GetComponent<HUDManager>();
        attack = 0;
        defense = 0;
    }


    // TODO: Find a way to remove this
    public override void Start()
    {

        base.Start();

    }


    public override IEnumerator DoTurn()
    {

        ProcessStatusEffects(); //Both
        isMyTurn = true;    
        
        deckHandler.ShowDeck();
        hudManager.UpdateTurnText(gameObject.name);
        UpdateHealthBar();

        

        while (isMyTurn)
        {
            yield return null;
        }

        UpdateHealthBar();
    }
    
    public void FinishTurn()
    {
        deckHandler.HideHand();
        isMyTurn = false;
    }

}
