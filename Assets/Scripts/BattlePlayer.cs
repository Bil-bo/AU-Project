using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattlePlayer : BaseBattleCharacter
{
    private int atk;
    private List<AbilityCard> originalDeck = new List<AbilityCard>();
    public List<AbilityCard> Deck = new List<AbilityCard>();
    public List<AbilityCard> CurrentHand = new List<AbilityCard>(2); // only two abilities
    public AbilityCard currentCard;
    private GameObject Card1;
    private GameObject Card2;
    public System.Action<BattleEnemy> currentSelectedAbilityAction;
   

    
    // Start is called before the first frame update
    public override void Start()
    {
        
        base.Start();
        Card1 = GameObject.Find("Card1");
        Card2 = GameObject.Find("Card2");
        Debug.Log(Card1.ToString());
        Debug.Log(Card2.ToString());

        InitializeDeck();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack(){

    }

    public override void Defend(){

    }
    public void UseAbility(AbilityCard card)
    {
         if (card != null)
    {
        card.ExecuteAbility(this);
        currentCard = card;
    }
    else
    {
        Debug.LogError("Attempted to use a null card.");
    }
    }

    private void DisableAllButtons()
    {
        Card1.GetComponent<UnityEngine.UI.Button>().interactable = false;
        Card2.GetComponent<UnityEngine.UI.Button>().interactable = false;
    }

    private void EnableAllButtons()
    {
        Card1.GetComponent<UnityEngine.UI.Button>().interactable = true;
        Card2.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }

    public void UpdateButtonAbilities()
    {
        EnableAllButtons();

        // Gets two random abilities
        DealAbilities();

        // Sets the text object of the card buttons to the name of the abilities 
        Card1.transform.Find("Text").GetComponent<TMP_Text>().text = CurrentHand[0].Name;
        Card2.transform.Find("Text").GetComponent<TMP_Text>().text = CurrentHand[1].Name;

        // Assign the abilities to the button's onClick events
        Card1.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        Card1.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => UseAbility(CurrentHand[0]));

        Card2.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        Card2.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => UseAbility(CurrentHand[1]));
    }

    public void OnTargetSelected(BattleEnemy targetCharacter)
    {
        if (currentSelectedAbilityAction != null)
        {
            currentSelectedAbilityAction.Invoke(targetCharacter);
            currentSelectedAbilityAction = null; // Clear the action after it's used
        }
    }

    public void DealAbilities()
    {
        Deck = new List<AbilityCard>(originalDeck); // Reset the deck to its original state.
        CurrentHand.Clear();

        if (Deck.Count < 2)
        {
            Debug.LogError("Not enough abilities in the deck to deal!");
            return;
        }

        // Randomly select two cards
        for (int i = 0; i < 2; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, Deck.Count);
            CurrentHand.Add(Deck[randomIndex]);
            Deck.RemoveAt(randomIndex);
        }
    }

    public override IEnumerator DoTurn(){

        ProcessStatusEffects(); //Both
        UpdateButtonAbilities(); //Player
        //isPlayerTurn = gameObject.CompareTag("Player");
        hudManager.UpdateTurnText(gameObject.name);
        UpdateHealthBar();


        Card1.SetActive(true);
        Card2.SetActive(true);

        while (isMyTurn)
        {
            yield return null;
        }
        Card1.SetActive(false);
        Card2.SetActive(false);


        

        UpdateHealthBar();
    }

    public void InitializeDeck()
    {
        Deck.Add(new AbilityCard
        {
            Name = "Melee Attack",
            Description = "Deal damage to the opponent.",
            Range = 1,
            ExecuteAbility = (attacker) =>
            {
                attacker.currentSelectedAbilityAction = (selectedTarget) =>
                {
                    int damage = 40; // Example damage value
                    selectedTarget.TakeDamage(damage);
                    Debug.Log(attacker.gameObject.name + " does " + damage + " damage to " + selectedTarget.gameObject.name);

                    // End turn after attack
                    attacker.isMyTurn = false;
                };
            }
        });

        Deck.Add(new AbilityCard
        {
            Name = "Ranged Attack",
            Description = "Deal damage to the opponent.",
            Range = 4,
            ExecuteAbility = (attacker) =>
            {
                attacker.currentSelectedAbilityAction = (selectedTarget) =>
                {
                    int damage = 20; // Example damage value
                    selectedTarget.TakeDamage(damage);
                    Debug.Log(attacker.gameObject.name + " does " + damage + " damage to " + selectedTarget.gameObject.name);

                     // End turn after attack
                     attacker.isMyTurn = false;
                };
            }
        });

        Deck.Add(new AbilityCard
        {
            Name = "Defend",
            Description = "Halve incoming damage next turn.",
            ExecuteAbility = (attacker) =>
            {
                attacker.currentSelectedAbilityAction = null;
                DisableAllButtons();
                attacker.ApplyStatusEffect(new StatusEffect
                {
                    Name = "Defensive Stance",
                    Duration = 1,
                    EffectType = EffectType.IncreaseDefense,
                    EffectValue = 2f //Defense multiplier
                });
                Debug.Log("Defend Pressed");
                DisplayText("Defense Up");
                attacker.isMyTurn = false;
                
            }
        });

        // ADD NEW CARDS HERE

        originalDeck = new List<AbilityCard>(Deck); // Store a copy of the original deck.
    }
}
