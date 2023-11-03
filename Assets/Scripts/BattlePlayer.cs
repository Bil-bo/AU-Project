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



    // Start is called before the first frame update

    private void CreateCanvas()
    {
        deckHandler = Instantiate(deckViewer, deckViewer.transform.position, Quaternion.identity).GetComponent<DeckHandler>();
        Debug.Log(deckHandler.ToString());

    }
    public override void Start()
    {

        base.Start();





    }

    void Update()
    {
    }

    public override void Attack()
    {

    }

    public override void Defend()
    {

    }

    public override IEnumerator DoTurn()
    {

        ProcessStatusEffects(); //Both
        isMyTurn = true;    
        
        Debug.Log("Made it here acc" + isMyTurn);
        if (deckHandler == null) {
            Debug.Log("NOthign there");
            CreateCanvas(); }
        Debug.Log("We Continue");
        deckHandler.ShowDeck(this);
        //isPlayerTurn = gameObject.CompareTag("Player");
        if (hudManager == null) { hudManager = GameObject.Find("Canvas").GetComponent<HUDManager>(); }
        hudManager.UpdateTurnText(gameObject.name);
        UpdateHealthBar();

        

        while (isMyTurn)
        {
            Debug.Log(isMyTurn + "Is it tho");
            yield return null;
        }




        UpdateHealthBar();
    }
    
    public void FinishTurn()
    {
        Debug.Log("Why are we still here");
        deckHandler.HideHand();
        isMyTurn = false;
    }

}
